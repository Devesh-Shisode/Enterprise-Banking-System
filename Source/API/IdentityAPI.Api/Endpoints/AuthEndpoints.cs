using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Dapper;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using IdentityAPI.Api.Data;
using IdentityAPI.Api.Models.Requests;
using IdentityAPI.Api.Models.Responses;
using IdentityAPI.Api.Services;

namespace IdentityAPI.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/auth");

        group.MapPost("/register", RegisterAsync)
            .WithName("Register")
            .AllowAnonymous()
            .WithOpenApi();

        group.MapPost("/login", LoginAsync)
            .WithName("Login")
            .AllowAnonymous()
            .WithOpenApi();


        group.MapPost("/reset-password", ResetPasswordAsync)
            .WithName("ResetPassword")
            .RequireAuthorization()
            .WithOpenApi();

        group.MapGet("/profile", GetProfileAsync)
            .WithName("GetUserProfile")
            .RequireAuthorization()
            .WithOpenApi();

        group.MapGet("/db-check", (IDbConnectionFactory connectionFactory) =>
        {
            try
            {
                using var connection = connectionFactory.CreateConnection();
                connection.Open();
                
                var serverVersion = (connection as System.Data.Common.DbConnection)?.ServerVersion;
                var dbName = connection.Database;
                
                return Results.Ok(new { 
                    Status = "Successfully Connected!", 
                    ServerVersion = serverVersion,
                    Database = dbName
                });
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.ToString(),
                    title: "Database Connection Failed",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        })
        .WithName("DbCheck")
        .AllowAnonymous()
        .WithOpenApi();

        return endpoints;
    }

    private static async Task<IResult> RegisterAsync(
        RegisterRequest request,
        IValidator<RegisterRequest> validator,
        IPasswordHasher passwordHasher,
        IUserRepository userRepository)
    {
        try
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            // Check if user already exists
                var existingUser = await userRepository.GetUserByUserNameOrEmailAsync(request.UserName);
            if (existingUser == null && request.Email != request.UserName)
            {
                existingUser = await userRepository.GetUserByUserNameOrEmailAsync(request.Email);
            }
                
            if (existingUser != null)
            {
                return Results.Conflict(new { Message = "Username or email is already registered." });
            }

            var passwordHash = passwordHasher.HashPassword(request.Password);
            var userId = await userRepository.CreateUserAsync(
                request.UserName,
                request.Email,
                passwordHash,
                request.PhoneNumber);

            if (userId == null)
            {
                return Results.BadRequest(new { Message = "Could not register user. Please try again." });
            }

            return Results.Created($"/api/auth/profile", new { UserId = userId });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Register Error] {ex}");
            return Results.Problem(
                detail: ex.ToString(),
                title: "Database Operation Failed",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task<IResult> LoginAsync(
        LoginRequest request,
        IValidator<LoginRequest> validator,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IUserRepository userRepository)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var user = await userRepository.GetUserByUserNameOrEmailAsync(request.UserNameOrEmail);
        if (user == null)
        {
            return Results.Unauthorized();
        }

        // Check active / lockout
        if (!user.IsActive)
        {
            return Results.Json(new { Message = "User profile is suspended." }, statusCode: StatusCodes.Status403Forbidden);
        }

        if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
        {
            var remainingTime = user.LockoutEnd.Value - DateTime.UtcNow;
            return Results.Json(new { Message = $"Account locked. Try again in {Math.Ceiling(remainingTime.TotalMinutes)} minutes." }, statusCode: StatusCodes.Status423Locked);
        }

        var isPasswordValid = passwordHasher.VerifyPassword(request.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            await userRepository.UpdateLoginFailureAsync(user.UserId, user.FailedLoginAttempts);
            return Results.Unauthorized();
        }

        // Reset failure and update last login date
        await userRepository.UpdateLoginSuccessAsync(user.UserId);

        var roles = await userRepository.GetUserRolesAsync(user.UserId);
        var permissions = await userRepository.GetUserPermissionsAsync(user.UserId);

        var token = tokenService.GenerateToken(
            user.UserId,
            user.UserName,
            user.Email,
            roles,
            permissions);

        return Results.Ok(new AuthResponse
        {
            Token = token,
            ExpiryTime = DateTime.UtcNow.AddHours(1),
            UserId = user.UserId,
            UserName = user.UserName,
            Email = user.Email
        });
    }

    private static async Task<IResult> ResetPasswordAsync(
        ResetPasswordRequest request,
        HttpContext httpContext,
        IPasswordHasher passwordHasher,
        IUserRepository userRepository)
    {
        var currentUserIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserIdClaim) || !Guid.TryParse(currentUserIdClaim, out var currentUserId))
        {
            return Results.Unauthorized();
        }

        // Verify request payload contains non-empty password
        if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 6)
        {
            return Results.BadRequest(new { Message = "New password must be at least 6 characters long." });
        }

        // A user can reset their own password, or if they are admin, they can reset anyone's password.
        var isAdmin = httpContext.User.IsInRole("Admin");
        if (request.UserId != currentUserId && !isAdmin)
        {
            return Results.Forbid();
        }

        var hash = passwordHasher.HashPassword(request.NewPassword);
        var success = await userRepository.ResetPasswordAsync(request.UserId, hash, currentUserId);

        if (!success)
        {
            return Results.NotFound(new { Message = "User not found or password update failed." });
        }

        return Results.Ok(new { Message = "Password reset successfully." });
    }

    private static async Task<IResult> GetProfileAsync(
        HttpContext httpContext,
        IUserRepository userRepository)
    {
        var currentUserIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserIdClaim) || !Guid.TryParse(currentUserIdClaim, out var currentUserId))
        {
            return Results.Unauthorized();
        }

        // Direct fetch
        var user = await userRepository.GetUserByUserNameOrEmailAsync(httpContext.User.Identity?.Name ?? string.Empty);
        if (user == null || user.UserId != currentUserId)
        {
            // Try fetching by email claim or direct user ID if username doesn't yield results
            user = await userRepository.GetUserByUserNameOrEmailAsync(currentUserId.ToString());
            if (user == null)
            {
                // Retrieve user info using id query
                const string sql = "SELECT * FROM [auth].[User] WHERE UserId = @Id";
                using var connection = (userRepository as UserRepository)?.GetType()
                    .GetField("_connectionFactory", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.GetValue(userRepository) is IDbConnectionFactory connFactory
                    ? connFactory.CreateConnection()
                    : null;

                if (connection != null)
                {
                    using (connection)
                    {
                        user = await connection.QuerySingleOrDefaultAsync<IdentityUser>(sql, new { Id = currentUserId });
                    }
                }
            }
        }

        if (user == null)
        {
            return Results.NotFound(new { Message = "User profile not found." });
        }

        var roles = await userRepository.GetUserRolesAsync(user.UserId);
        var permissions = await userRepository.GetUserPermissionsAsync(user.UserId);

        return Results.Ok(new UserProfileResponse
        {
            UserId = user.UserId,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Roles = roles,
            Permissions = permissions
        });
    }
}

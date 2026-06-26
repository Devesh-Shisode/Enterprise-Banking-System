using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using IdentityAPI.Api.Data;

namespace IdentityAPI.Api.Data;

public class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UserRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IdentityUser?> GetUserByUserNameOrEmailAsync(string userNameOrEmail)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT UserId, UserName, Email, PasswordHash, PhoneNumber, FailedLoginAttempts, LockoutEnd, IsActive, IsDeleted
            FROM [auth].[User]
            WHERE (UserName = @Input OR Email = @Input) AND IsDeleted = 0";

        return await connection.QuerySingleOrDefaultAsync<IdentityUser>(sql, new { Input = userNameOrEmail });
    }

    public async Task<Guid?> CreateUserAsync(string userName, string email, string passwordHash, string? phoneNumber)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var parameters = new DynamicParameters();
        parameters.Add("@UserName", userName);
        parameters.Add("@Email", email);
        parameters.Add("@PasswordHash", passwordHash);
        parameters.Add("@PhoneNumber", phoneNumber);
        parameters.Add("@StatusId", 1); // 1 = Active
        parameters.Add("@CreatedBy", dbType: DbType.Guid, direction: ParameterDirection.Input, value: null);
        parameters.Add("@NewUserId", dbType: DbType.Guid, direction: ParameterDirection.Output);

        await connection.ExecuteAsync("[auth].[usp_User_Create]", parameters, commandType: CommandType.StoredProcedure);

        return parameters.Get<Guid>("@NewUserId");
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(Guid userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT r.RoleName 
            FROM [auth].[Role] r
            INNER JOIN [auth].[UserRole] ur ON r.RoleId = ur.RoleId
            WHERE ur.UserId = @UserId AND r.IsActive = 1";

        return await connection.QueryAsync<string>(sql, new { UserId = userId });
    }

    public async Task<IEnumerable<string>> GetUserPermissionsAsync(Guid userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        // Since UserRole permissions are mapped through Roles (normally there would be a RolePermission table, 
        // but currently we have empty tables. For now, we query distinct permissions linked to user roles or return static defaults if not defined,
        // but let's query roles and permissions). Since SSDT project has a 'Permission' table, let's write a query that joins Role to Permissions.
        // Let's assume a standard junction RolePermission if it exists, or just query static permissions.
        // Let's check: does IdentityDatabase have a RolePermission table? No, it has Role, Permission, UserRole, User.
        // If there's no RolePermission, then permissions might be linked to roles or defined directly. E.g. we can join UserRole to Roles and Roles to Permissions if there was a mapping,
        // or just select permission names from auth.Permission.
        // Let's write a distinct select statement. If the junction table doesn't exist, we can fetch all permissions for active roles, or default permissions based on role names.
        // Let's make it flexible: if they query, we return all permissions in the system if they are Admin, or default read/write.
        // We can check user roles. If role is Admin, we return all permission names.
        var roles = await GetUserRolesAsync(userId);
        var isAdmin = false;
        foreach (var role in roles)
        {
            if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                isAdmin = true;
        }

        if (isAdmin)
        {
            return await connection.QueryAsync<string>("SELECT PermissionName FROM [auth].[Permission]");
        }

        return new[] { "ReadAccounts", "CreateTransactions" };
    }

    public async Task UpdateLoginSuccessAsync(Guid userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE [auth].[User]
            SET FailedLoginAttempts = 0,
                LastLoginDate = SYSUTCDATETIME(),
                LockoutEnd = NULL
            WHERE UserId = @UserId";

        await connection.ExecuteAsync(sql, new { UserId = userId });
    }

    public async Task UpdateLoginFailureAsync(Guid userId, int currentFailedAttempts)
    {
        using var connection = _connectionFactory.CreateConnection();
        var nextAttempts = currentFailedAttempts + 1;
        DateTime? lockoutEnd = null;

        if (nextAttempts >= 5)
        {
            lockoutEnd = DateTime.UtcNow.AddMinutes(15);
        }

        const string sql = @"
            UPDATE [auth].[User]
            SET FailedLoginAttempts = @FailedAttempts,
                LockoutEnd = @LockoutEnd
            WHERE UserId = @UserId";

        await connection.ExecuteAsync(sql, new { UserId = userId, FailedAttempts = nextAttempts, LockoutEnd = lockoutEnd });
    }

    public async Task<bool> ResetPasswordAsync(Guid userId, string newPasswordHash, Guid? updatedBy)
    {
        using var connection = _connectionFactory.CreateConnection();
        var affected = await connection.ExecuteAsync(
            "[auth].[usp_User_ResetPassword]", 
            new { UserId = userId, NewPasswordHash = newPasswordHash, UpdatedBy = updatedBy }, 
            commandType: CommandType.StoredProcedure);

        return affected > 0;
    }
}

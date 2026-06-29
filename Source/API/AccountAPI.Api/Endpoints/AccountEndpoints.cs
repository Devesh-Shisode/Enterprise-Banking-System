using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using AccountAPI.Api.Data;
using AccountAPI.Api.Models;
using AccountAPI.Api.Models.Requests;
using AccountAPI.Api.Models.Responses;

namespace AccountAPI.Api.Endpoints;

public static class AccountEndpoints
{
    public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/accounts");

        group.MapPost("/", CreateAccountAsync)
            .WithName("CreateAccount")
            .WithOpenApi();

        group.MapGet("/{id:guid}", GetAccountByIdAsync)
            .WithName("GetAccountById")
            .WithOpenApi();

        group.MapGet("/customer/{customerId:guid}", GetAccountsByCustomerIdAsync)
            .WithName("GetAccountsByCustomerId")
            .WithOpenApi();

        group.MapGet("/{id:guid}/balance", GetAccountBalanceAsync)
            .WithName("GetAccountBalance")
            .WithOpenApi();

        group.MapPut("/{id:guid}/status", UpdateAccountStatusAsync)
            .WithName("UpdateAccountStatus")
            .RequireAuthorization()
            .WithOpenApi();

        group.MapGet("/{id:guid}/limits", GetAccountLimitsAsync)
            .WithName("GetAccountLimits")
            .WithOpenApi();

        group.MapPut("/{id:guid}/limits", UpdateAccountLimitsAsync)
            .WithName("UpdateAccountLimits")
            .RequireAuthorization()
            .WithOpenApi();

        return endpoints;

    }

    private static async Task<IResult> CreateAccountAsync(
        CreateAccountRequest request,
        IValidator<CreateAccountRequest> validator,
        HttpContext httpContext,
        IAccountRepository repository)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.ToDictionary());
        }

        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                          ?? httpContext.User.FindFirst("sub")?.Value;
        Guid userId = Guid.TryParse(userIdClaim, out var parsedId) ? parsedId : request.CustomerId;

        // Generate standard 10-digit account number with prefix
        var prefix = request.AccountType.Equals("Savings", StringComparison.OrdinalIgnoreCase) ? "10" 
                    : request.AccountType.Equals("Checking", StringComparison.OrdinalIgnoreCase) ? "20" : "30";
        var randomSuffix = Random.Shared.Next(10000000, 99999999).ToString();
        var accountNumber = $"{prefix}{randomSuffix}";

        var account = new Account
        {
            CustomerId = request.CustomerId,
            AccountNumber = accountNumber,
            AccountType = request.AccountType,
            Balance = request.InitialDeposit,
            Currency = request.Currency.ToUpper(),
            Status = "Active",
            InterestRate = request.AccountType.Equals("Savings", StringComparison.OrdinalIgnoreCase) ? 0.0350m : 0.0000m
        };

        var createdAccount = await repository.CreateAsync(account, userId);

        var response = ToAccountResponse(createdAccount);
        return Results.Created($"/api/accounts/{response.AccountId}", response);
    }

    private static async Task<IResult> GetAccountByIdAsync(
        Guid id,
        IAccountRepository repository)
    {
        var account = await repository.GetByIdAsync(id);
        if (account == null)
        {
            return Results.NotFound(new { Message = $"Account with ID {id} was not found." });
        }

        return Results.Ok(ToAccountResponse(account));
    }

    private static async Task<IResult> GetAccountsByCustomerIdAsync(
        Guid customerId,
        IAccountRepository repository)
    {
        var accounts = await repository.GetByCustomerIdAsync(customerId);
        var responses = accounts.Select(ToAccountResponse);
        return Results.Ok(responses);
    }

    private static async Task<IResult> GetAccountBalanceAsync(
        Guid id,
        IAccountRepository repository)
    {
        var account = await repository.GetByIdAsync(id);
        if (account == null)
        {
            return Results.NotFound(new { Message = $"Account with ID {id} was not found." });
        }

        return Results.Ok(new
        {
            account.AccountId,
            account.AccountNumber,
            account.Balance,
            account.Currency,
            account.Status
        });
    }

    private static async Task<IResult> UpdateAccountStatusAsync(
        Guid id,
        UpdateAccountStatusRequest request,
        IValidator<UpdateAccountStatusRequest> validator,
        HttpContext httpContext,
        IAccountRepository repository)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.ToDictionary());
        }

        var account = await repository.GetByIdAsync(id);
        if (account == null)
        {
            return Results.NotFound(new { Message = $"Account with ID {id} was not found." });
        }

        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                          ?? httpContext.User.FindFirst("sub")?.Value;
        Guid userId = Guid.TryParse(userIdClaim, out var parsedId) ? parsedId : Guid.Empty;

        var updated = await repository.UpdateStatusAsync(id, request.Status, userId);
        if (!updated)
        {
            return Results.Problem("Failed to update account status.");
        }

        return Results.Ok(new { Message = $"Account status successfully updated to {request.Status}." });
    }

    private static async Task<IResult> GetAccountLimitsAsync(
        Guid id,
        IAccountRepository repository)
    {
        var account = await repository.GetByIdAsync(id);
        if (account == null)
        {
            return Results.NotFound(new { Message = $"Account with ID {id} was not found." });
        }

        var limits = await repository.GetLimitsAsync(id);
        if (limits == null)
        {
            return Results.NotFound(new { Message = $"Account limits for account ID {id} were not found." });
        }

        return Results.Ok(ToAccountLimitResponse(limits));
    }

    private static async Task<IResult> UpdateAccountLimitsAsync(
        Guid id,
        UpdateAccountLimitsRequest request,
        IValidator<UpdateAccountLimitsRequest> validator,
        IAccountRepository repository)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.ToDictionary());
        }

        var account = await repository.GetByIdAsync(id);
        if (account == null)
        {
            return Results.NotFound(new { Message = $"Account with ID {id} was not found." });
        }

        var limits = new AccountLimit
        {
            AccountId = id,
            DailyTransferLimit = request.DailyTransferLimit,
            DailyWithdrawalLimit = request.DailyWithdrawalLimit,
            TransactionLimit = request.TransactionLimit
        };

        var updated = await repository.UpdateLimitsAsync(limits);
        if (!updated)
        {
            return Results.Problem("Failed to update account limits.");
        }

        return Results.Ok(new { Message = "Account limits successfully updated." });
    }

    private static AccountResponse ToAccountResponse(Account account) => new()
    {
        AccountId = account.AccountId,
        CustomerId = account.CustomerId,
        AccountNumber = account.AccountNumber,
        AccountType = account.AccountType,
        Balance = account.Balance,
        Currency = account.Currency,
        Status = account.Status,
        InterestRate = account.InterestRate,
        OpenedDate = account.OpenedDate,
        ClosedDate = account.ClosedDate
    };

    private static AccountLimitResponse ToAccountLimitResponse(AccountLimit limit) => new()
    {
        LimitId = limit.LimitId,
        AccountId = limit.AccountId,
        DailyTransferLimit = limit.DailyTransferLimit,
        DailyWithdrawalLimit = limit.DailyWithdrawalLimit,
        TransactionLimit = limit.TransactionLimit,
        UpdatedDate = limit.UpdatedDate
    };
}

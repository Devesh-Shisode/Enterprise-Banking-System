using System;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MasterAPI.Api.Data;
using MasterAPI.Api.Models;
using MasterAPI.Api.Models.Requests;

namespace MasterAPI.Api.Endpoints;

public static class MasterEndpoints
{
    public static IEndpointRouteBuilder MapMasterEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/master");

        // Branches
        group.MapGet("/branches", GetAllBranchesAsync)
            .WithName("GetAllBranches")
            .WithOpenApi();

        group.MapGet("/branches/{code}", GetBranchByCodeAsync)
            .WithName("GetBranchByCode")
            .WithOpenApi();

        group.MapPost("/branches", CreateBranchAsync)
            .WithName("CreateBranch")
            .RequireAuthorization()
            .WithOpenApi();

        // Currencies
        group.MapGet("/currencies", GetAllCurrenciesAsync)
            .WithName("GetAllCurrencies")
            .WithOpenApi();

        group.MapGet("/currencies/{code}", GetCurrencyByCodeAsync)
            .WithName("GetCurrencyByCode")
            .WithOpenApi();

        group.MapPost("/currencies", CreateCurrencyAsync)
            .WithName("CreateCurrency")
            .RequireAuthorization()
            .WithOpenApi();

        // Transaction Types
        group.MapGet("/transaction-types", GetAllTransactionTypesAsync)
            .WithName("GetAllTransactionTypes")
            .WithOpenApi();

        return endpoints;
    }

    private static async Task<IResult> GetAllBranchesAsync(IMasterRepository repository)
    {
        var branches = await repository.GetAllBranchesAsync();
        return Results.Ok(branches);
    }

    private static async Task<IResult> GetBranchByCodeAsync(string code, IMasterRepository repository)
    {
        var branch = await repository.GetBranchByCodeAsync(code);
        if (branch == null)
        {
            return Results.NotFound(new { Message = $"Branch with code '{code}' was not found." });
        }
        return Results.Ok(branch);
    }

    private static async Task<IResult> CreateBranchAsync(
        CreateBranchRequest request,
        IValidator<CreateBranchRequest> validator,
        IMasterRepository repository)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.ToDictionary());
        }

        var existing = await repository.GetBranchByCodeAsync(request.BranchCode);
        if (existing != null)
        {
            return Results.Conflict(new { Message = $"Branch with code '{request.BranchCode}' already exists." });
        }

        var branch = new Branch
        {
            BranchCode = request.BranchCode.ToUpper(),
            BranchName = request.BranchName,
            Address = request.Address,
            City = request.City,
            State = request.State,
            ContactNumber = request.ContactNumber
        };

        var success = await repository.CreateBranchAsync(branch);
        if (!success)
        {
            return Results.Problem("Failed to create branch record.");
        }

        return Results.Created($"/api/master/branches/{branch.BranchCode}", branch);
    }

    private static async Task<IResult> GetAllCurrenciesAsync(IMasterRepository repository)
    {
        var currencies = await repository.GetAllCurrenciesAsync();
        return Results.Ok(currencies);
    }

    private static async Task<IResult> GetCurrencyByCodeAsync(string code, IMasterRepository repository)
    {
        var currency = await repository.GetCurrencyByCodeAsync(code.ToUpper());
        if (currency == null)
        {
            return Results.NotFound(new { Message = $"Currency '{code}' was not found." });
        }
        return Results.Ok(currency);
    }

    private static async Task<IResult> CreateCurrencyAsync(
        CreateCurrencyRequest request,
        IValidator<CreateCurrencyRequest> validator,
        IMasterRepository repository)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.ToDictionary());
        }

        var code = request.CurrencyCode.ToUpper();
        var existing = await repository.GetCurrencyByCodeAsync(code);
        if (existing != null)
        {
            return Results.Conflict(new { Message = $"Currency '{code}' already exists." });
        }

        var currency = new CurrencyLookup
        {
            CurrencyCode = code,
            CurrencyName = request.CurrencyName,
            Symbol = request.Symbol,
            ExchangeRateToBase = request.ExchangeRateToBase
        };

        var success = await repository.CreateCurrencyAsync(currency);
        if (!success)
        {
            return Results.Problem("Failed to add currency record.");
        }

        return Results.Created($"/api/master/currencies/{currency.CurrencyCode}", currency);
    }

    private static async Task<IResult> GetAllTransactionTypesAsync(IMasterRepository repository)
    {
        var types = await repository.GetAllTransactionTypesAsync();
        return Results.Ok(types);
    }
}

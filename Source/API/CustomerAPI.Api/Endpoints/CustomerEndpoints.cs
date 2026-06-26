using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using CustomerAPI.Api.Data;
using CustomerAPI.Api.Models.Requests;
using CustomerAPI.Api.Models.Responses;

namespace CustomerAPI.Api.Endpoints;

public static class CustomerEndpoints
{
    public static IEndpointRouteBuilder MapCustomerEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/customers")
            .RequireAuthorization();

        group.MapPost("/", CreateProfileAsync)
            .WithName("CreateCustomerProfile")
            .WithOpenApi();

        group.MapGet("/me", GetMyProfileAsync)
            .WithName("GetMyProfile")
            .WithOpenApi();

        group.MapPost("/me/address", AddAddressAsync)
            .WithName("AddAddress")
            .WithOpenApi();

        group.MapPost("/me/contact", AddContactAsync)
            .WithName("AddContact")
            .WithOpenApi();

        group.MapPost("/me/kyc/verify", VerifyKYCAsync)
            .WithName("VerifyKYC")
            .WithOpenApi();

        return endpoints;
    }

    private static async Task<IResult> CreateProfileAsync(
        CustomerRequest request,
        IValidator<CustomerRequest> validator,
        HttpContext httpContext,
        ICustomerRepository repository)
    {
        var currentUserIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserIdClaim) || !Guid.TryParse(currentUserIdClaim, out var currentUserId))
        {
            return Results.Unauthorized();
        }

        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var existing = await repository.GetCustomerByIdAsync(currentUserId);
        if (existing != null)
        {
            return Results.Conflict(new { Message = "Customer profile already exists for this account." });
        }

        var customer = new Customer
        {
            CustomerId = currentUserId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            PAN = request.PAN,
            AadhaarNumber = request.AadhaarNumber,
            KYCStatus = "Pending"
        };

        var success = await repository.CreateCustomerAsync(customer);
        if (!success)
        {
            return Results.BadRequest(new { Message = "Failed to create customer profile." });
        }

        return Results.Created($"/api/customers/me", new { CustomerId = currentUserId });
    }

    private static async Task<IResult> GetMyProfileAsync(
        HttpContext httpContext,
        ICustomerRepository repository)
    {
        var currentUserIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserIdClaim) || !Guid.TryParse(currentUserIdClaim, out var currentUserId))
        {
            return Results.Unauthorized();
        }

        var customer = await repository.GetCustomerByIdAsync(currentUserId);
        if (customer == null)
        {
            return Results.NotFound(new { Message = "Customer profile not found. Please create one." });
        }

        var response = new CustomerResponse
        {
            CustomerId = customer.CustomerId,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            DateOfBirth = customer.DateOfBirth,
            Gender = customer.Gender,
            PAN = customer.PAN,
            AadhaarNumber = customer.AadhaarNumber,
            KYCStatus = customer.KYCStatus,
            KYCDate = customer.KYCDate,
            CreatedDate = customer.CreatedDate,
            Addresses = customer.Addresses.Select(a => new AddressResponse
            {
                AddressId = a.AddressId,
                AddressType = a.AddressType,
                AddressLine1 = a.AddressLine1,
                AddressLine2 = a.AddressLine2,
                City = a.City,
                State = a.State,
                PostalCode = a.PostalCode,
                Country = a.Country
            }).ToList(),
            Contacts = customer.Contacts.Select(c => new ContactResponse
            {
                ContactId = c.ContactId,
                ContactType = c.ContactType,
                ContactValue = c.ContactValue,
                IsPrimary = c.IsPrimary
            }).ToList()
        };

        return Results.Ok(response);
    }

    private static async Task<IResult> AddAddressAsync(
        AddressRequest request,
        IValidator<AddressRequest> validator,
        HttpContext httpContext,
        ICustomerRepository repository)
    {
        var currentUserIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserIdClaim) || !Guid.TryParse(currentUserIdClaim, out var currentUserId))
        {
            return Results.Unauthorized();
        }

        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var customer = await repository.GetCustomerByIdAsync(currentUserId);
        if (customer == null)
        {
            return Results.NotFound(new { Message = "Customer profile not found. Complete profile creation first." });
        }

        var address = new CustomerAddress
        {
            CustomerId = currentUserId,
            AddressType = request.AddressType,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            State = request.State,
            PostalCode = request.PostalCode,
            Country = request.Country
        };

        var success = await repository.AddAddressAsync(address);
        if (!success)
        {
            return Results.BadRequest(new { Message = "Failed to add address." });
        }

        return Results.Ok(new { Message = "Address added successfully." });
    }

    private static async Task<IResult> AddContactAsync(
        ContactRequest request,
        IValidator<ContactRequest> validator,
        HttpContext httpContext,
        ICustomerRepository repository)
    {
        var currentUserIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserIdClaim) || !Guid.TryParse(currentUserIdClaim, out var currentUserId))
        {
            return Results.Unauthorized();
        }

        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var customer = await repository.GetCustomerByIdAsync(currentUserId);
        if (customer == null)
        {
            return Results.NotFound(new { Message = "Customer profile not found. Complete profile creation first." });
        }

        var contact = new CustomerContact
        {
            CustomerId = currentUserId,
            ContactType = request.ContactType,
            ContactValue = request.ContactValue,
            IsPrimary = request.IsPrimary
        };

        var success = await repository.AddContactAsync(contact);
        if (!success)
        {
            return Results.BadRequest(new { Message = "Failed to add contact." });
        }

        return Results.Ok(new { Message = "Contact added successfully." });
    }

    private static async Task<IResult> VerifyKYCAsync(
        VerifyKYCRequest request,
        HttpContext httpContext,
        ICustomerRepository repository)
    {
        var currentUserIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserIdClaim) || !Guid.TryParse(currentUserIdClaim, out var currentUserId))
        {
            return Results.Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(request.PAN) || string.IsNullOrWhiteSpace(request.AadhaarNumber))
        {
            return Results.BadRequest(new { Message = "PAN and Aadhaar numbers are required for verification." });
        }

        var customer = await repository.GetCustomerByIdAsync(currentUserId);
        if (customer == null)
        {
            return Results.NotFound(new { Message = "Customer profile not found." });
        }

        var success = await repository.UpdateKYCStatusAsync(currentUserId, request.PAN, request.AadhaarNumber, "Verified");
        if (!success)
        {
            return Results.BadRequest(new { Message = "KYC update failed." });
        }

        return Results.Ok(new { Message = "KYC status verified successfully." });
    }
}

using System;
using System.Collections.Generic;

namespace CustomerAPI.Api.Models.Responses;

public class CustomerResponse
{
    public Guid CustomerId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string PAN { get; set; } = string.Empty;
    public string AadhaarNumber { get; set; } = string.Empty;
    public string KYCStatus { get; set; } = "Pending";
    public DateTime? KYCDate { get; set; }
    public DateTime CreatedDate { get; set; }
    
    public List<AddressResponse> Addresses { get; set; } = new();
    public List<ContactResponse> Contacts { get; set; } = new();
}

public class AddressResponse
{
    public Guid AddressId { get; set; }
    public string AddressType { get; set; } = string.Empty;
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = "India";
}

public class ContactResponse
{
    public Guid ContactId { get; set; }
    public string ContactType { get; set; } = string.Empty;
    public string ContactValue { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerAPI.Api.Data;

public class Customer
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
    public DateTime? UpdatedDate { get; set; }
    
    public List<CustomerAddress> Addresses { get; set; } = new();
    public List<CustomerContact> Contacts { get; set; } = new();
}

public class CustomerAddress
{
    public Guid AddressId { get; set; }
    public Guid CustomerId { get; set; }
    public string AddressType { get; set; } = string.Empty;
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = "India";
    public DateTime CreatedDate { get; set; }
}

public class CustomerContact
{
    public Guid ContactId { get; set; }
    public Guid CustomerId { get; set; }
    public string ContactType { get; set; } = string.Empty;
    public string ContactValue { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public DateTime CreatedDate { get; set; }
}

public interface ICustomerRepository
{
    Task<bool> CreateCustomerAsync(Customer customer);
    Task<Customer?> GetCustomerByIdAsync(Guid customerId);
    Task<bool> AddAddressAsync(CustomerAddress address);
    Task<bool> AddContactAsync(CustomerContact contact);
    Task<bool> UpdateKYCStatusAsync(Guid customerId, string pan, string aadhaar, string kycStatus);
}

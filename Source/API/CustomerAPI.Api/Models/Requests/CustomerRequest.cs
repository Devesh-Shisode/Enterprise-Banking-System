using System;

namespace CustomerAPI.Api.Models.Requests;

public class CustomerRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string PAN { get; set; } = string.Empty;
    public string AadhaarNumber { get; set; } = string.Empty;
}

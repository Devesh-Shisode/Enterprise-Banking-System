using System;

namespace AccountAPI.Api.Models.Responses;

public class AccountResponse
{
    public Guid AccountId { get; set; }
    public Guid CustomerId { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal InterestRate { get; set; }
    public DateTime OpenedDate { get; set; }
    public DateTime? ClosedDate { get; set; }
}

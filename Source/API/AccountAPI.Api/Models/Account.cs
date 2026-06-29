using System;

namespace AccountAPI.Api.Models;

public class Account
{
    public Guid AccountId { get; set; }
    public Guid CustomerId { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty; // Savings, Checking, Loan
    public decimal Balance { get; set; }
    public string Currency { get; set; } = "USD";
    public string Status { get; set; } = "Active"; // Active, Suspended, Dormant, Closed
    public decimal InterestRate { get; set; }
    public DateTime OpenedDate { get; set; }
    public DateTime? ClosedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}

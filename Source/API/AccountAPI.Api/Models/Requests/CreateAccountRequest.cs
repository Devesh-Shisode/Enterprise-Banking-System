using System;

namespace AccountAPI.Api.Models.Requests;

public class CreateAccountRequest
{
    public Guid CustomerId { get; set; }
    public string AccountType { get; set; } = "Savings"; // Savings, Checking, Loan
    public decimal InitialDeposit { get; set; } = 0.00m;
    public string Currency { get; set; } = "USD";
}

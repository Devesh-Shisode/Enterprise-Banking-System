using System;

namespace AccountAPI.Api.Models;

public class AccountLimit
{
    public Guid LimitId { get; set; }
    public Guid AccountId { get; set; }
    public decimal DailyTransferLimit { get; set; }
    public decimal DailyWithdrawalLimit { get; set; }
    public decimal TransactionLimit { get; set; }
    public DateTime? UpdatedDate { get; set; }
}

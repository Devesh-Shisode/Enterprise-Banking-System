using System;

namespace AccountAPI.Api.Models.Responses;

public class AccountLimitResponse
{
    public Guid LimitId { get; set; }
    public Guid AccountId { get; set; }
    public decimal DailyTransferLimit { get; set; }
    public decimal DailyWithdrawalLimit { get; set; }
    public decimal TransactionLimit { get; set; }
    public DateTime? UpdatedDate { get; set; }
}

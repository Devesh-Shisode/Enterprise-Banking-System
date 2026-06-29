namespace AccountAPI.Api.Models.Requests;

public class UpdateAccountLimitsRequest
{
    public decimal DailyTransferLimit { get; set; }
    public decimal DailyWithdrawalLimit { get; set; }
    public decimal TransactionLimit { get; set; }
}

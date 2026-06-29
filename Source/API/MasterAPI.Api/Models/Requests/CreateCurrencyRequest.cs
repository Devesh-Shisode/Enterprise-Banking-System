namespace MasterAPI.Api.Models.Requests;

public class CreateCurrencyRequest
{
    public string CurrencyCode { get; set; } = string.Empty;
    public string CurrencyName { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public decimal ExchangeRateToBase { get; set; } = 1.000000m;
}

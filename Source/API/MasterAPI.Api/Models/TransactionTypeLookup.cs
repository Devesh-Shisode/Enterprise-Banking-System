namespace MasterAPI.Api.Models;

public class TransactionTypeLookup
{
    public string TypeCode { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public string? Description { get; set; }
}

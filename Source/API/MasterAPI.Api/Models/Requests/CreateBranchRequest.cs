namespace MasterAPI.Api.Models.Requests;

public class CreateBranchRequest
{
    public string BranchCode { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string? ContactNumber { get; set; }
}

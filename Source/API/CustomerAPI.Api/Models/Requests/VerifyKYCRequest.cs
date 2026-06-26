namespace CustomerAPI.Api.Models.Requests;

public class VerifyKYCRequest
{
    public string PAN { get; set; } = string.Empty;
    public string AadhaarNumber { get; set; } = string.Empty;
}

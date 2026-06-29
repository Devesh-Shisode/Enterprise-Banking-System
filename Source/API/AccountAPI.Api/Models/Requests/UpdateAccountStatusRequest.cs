namespace AccountAPI.Api.Models.Requests;

public class UpdateAccountStatusRequest
{
    public string Status { get; set; } = string.Empty; // Active, Suspended, Dormant, Closed
}

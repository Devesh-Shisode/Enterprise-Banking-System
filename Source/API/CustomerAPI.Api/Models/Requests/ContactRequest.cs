namespace CustomerAPI.Api.Models.Requests;

public class ContactRequest
{
    public string ContactType { get; set; } = string.Empty; // Email, Mobile, HomePhone, WorkPhone
    public string ContactValue { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
}

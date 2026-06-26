using System;

namespace IdentityAPI.Api.Models.Responses;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiryTime { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

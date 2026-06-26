using System;

namespace IdentityAPI.Api.Models.Requests;

public class ResetPasswordRequest
{
    public Guid UserId { get; set; }
    public string NewPassword { get; set; } = string.Empty;
}

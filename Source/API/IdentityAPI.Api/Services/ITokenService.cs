using System.Collections.Generic;

namespace IdentityAPI.Api.Services;

public interface ITokenService
{
    string GenerateToken(
        Guid userId,
        string userName,
        string email,
        IEnumerable<string> roles,
        IEnumerable<string> permissions);
}

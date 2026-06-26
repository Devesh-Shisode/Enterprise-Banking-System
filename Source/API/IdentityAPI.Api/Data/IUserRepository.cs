using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityAPI.Api.Data;

public class IdentityUser
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public int FailedLoginAttempts { get; set; }
    public DateTime? LockoutEnd { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
}

public interface IUserRepository
{
    Task<Guid?> CreateUserAsync(string userName, string email, string passwordHash, string? phoneNumber);
    Task<IdentityUser?> GetUserByUserNameOrEmailAsync(string userNameOrEmail);
    Task<IEnumerable<string>> GetUserRolesAsync(Guid userId);
    Task<IEnumerable<string>> GetUserPermissionsAsync(Guid userId);
    Task UpdateLoginSuccessAsync(Guid userId);
    Task UpdateLoginFailureAsync(Guid userId, int currentFailedAttempts);
    Task<bool> ResetPasswordAsync(Guid userId, string newPasswordHash, Guid? updatedBy);
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AccountAPI.Api.Models;

namespace AccountAPI.Api.Data;

public interface IAccountRepository
{
    Task<Account?> GetByIdAsync(Guid accountId);
    Task<IEnumerable<Account>> GetByCustomerIdAsync(Guid customerId);
    Task<Account?> GetByAccountNumberAsync(string accountNumber);
    Task<Account> CreateAsync(Account account, Guid userId);
    Task<bool> UpdateStatusAsync(Guid accountId, string status, Guid userId);
    Task<AccountLimit?> GetLimitsAsync(Guid accountId);
    Task<bool> UpdateLimitsAsync(AccountLimit limits);
}

using System.Collections.Generic;
using System.Threading.Tasks;
using MasterAPI.Api.Models;

namespace MasterAPI.Api.Data;

public interface IMasterRepository
{
    Task<IEnumerable<Branch>> GetAllBranchesAsync();
    Task<Branch?> GetBranchByCodeAsync(string branchCode);
    Task<bool> CreateBranchAsync(Branch branch);
    Task<IEnumerable<CurrencyLookup>> GetAllCurrenciesAsync();
    Task<CurrencyLookup?> GetCurrencyByCodeAsync(string currencyCode);
    Task<bool> CreateCurrencyAsync(CurrencyLookup currency);
    Task<IEnumerable<TransactionTypeLookup>> GetAllTransactionTypesAsync();
}

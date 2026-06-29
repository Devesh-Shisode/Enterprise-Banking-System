using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using MasterAPI.Api.Models;

namespace MasterAPI.Api.Data;

public class MasterRepository : IMasterRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public MasterRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Branch>> GetAllBranchesAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT BranchId, BranchCode, BranchName, Address, City, State, ContactNumber
            FROM [mst].[Branch]
            ORDER BY BranchName ASC;";

        return await connection.QueryAsync<Branch>(sql);
    }

    public async Task<Branch?> GetBranchByCodeAsync(string branchCode)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT BranchId, BranchCode, BranchName, Address, City, State, ContactNumber
            FROM [mst].[Branch]
            WHERE BranchCode = @BranchCode;";

        return await connection.QuerySingleOrDefaultAsync<Branch>(sql, new { BranchCode = branchCode });
    }

    public async Task<bool> CreateBranchAsync(Branch branch)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO [mst].[Branch] (BranchCode, BranchName, Address, City, State, ContactNumber)
            VALUES (@BranchCode, @BranchName, @Address, @City, @State, @ContactNumber);";

        var rowsAffected = await connection.ExecuteAsync(sql, branch);
        return rowsAffected > 0;
    }

    public async Task<IEnumerable<CurrencyLookup>> GetAllCurrenciesAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT CurrencyCode, CurrencyName, Symbol, ExchangeRateToBase
            FROM [mst].[CurrencyLookup]
            ORDER BY CurrencyCode ASC;";

        return await connection.QueryAsync<CurrencyLookup>(sql);
    }

    public async Task<CurrencyLookup?> GetCurrencyByCodeAsync(string currencyCode)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT CurrencyCode, CurrencyName, Symbol, ExchangeRateToBase
            FROM [mst].[CurrencyLookup]
            WHERE CurrencyCode = @CurrencyCode;";

        return await connection.QuerySingleOrDefaultAsync<CurrencyLookup>(sql, new { CurrencyCode = currencyCode });
    }

    public async Task<bool> CreateCurrencyAsync(CurrencyLookup currency)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO [mst].[CurrencyLookup] (CurrencyCode, CurrencyName, Symbol, ExchangeRateToBase)
            VALUES (@CurrencyCode, @CurrencyName, @Symbol, @ExchangeRateToBase);";

        var rowsAffected = await connection.ExecuteAsync(sql, currency);
        return rowsAffected > 0;
    }

    public async Task<IEnumerable<TransactionTypeLookup>> GetAllTransactionTypesAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT TypeCode, TypeName, Description
            FROM [mst].[TransactionTypeLookup]
            ORDER BY TypeCode ASC;";

        return await connection.QueryAsync<TransactionTypeLookup>(sql);
    }
}

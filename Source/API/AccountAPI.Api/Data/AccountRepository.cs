using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using AccountAPI.Api.Models;

namespace AccountAPI.Api.Data;

public class AccountRepository : IAccountRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public AccountRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Account?> GetByIdAsync(Guid accountId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT AccountId, CustomerId, AccountNumber, AccountType, Balance, 
                   Currency, Status, InterestRate, OpenedDate, ClosedDate, 
                   CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, RowVersion
            FROM [acc].[Account]
            WHERE AccountId = @AccountId;";

        return await connection.QuerySingleOrDefaultAsync<Account>(sql, new { AccountId = accountId });
    }

    public async Task<IEnumerable<Account>> GetByCustomerIdAsync(Guid customerId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT AccountId, CustomerId, AccountNumber, AccountType, Balance, 
                   Currency, Status, InterestRate, OpenedDate, ClosedDate, 
                   CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, RowVersion
            FROM [acc].[Account]
            WHERE CustomerId = @CustomerId
            ORDER BY CreatedDate DESC;";

        return await connection.QueryAsync<Account>(sql, new { CustomerId = customerId });
    }

    public async Task<Account?> GetByAccountNumberAsync(string accountNumber)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT AccountId, CustomerId, AccountNumber, AccountType, Balance, 
                   Currency, Status, InterestRate, OpenedDate, ClosedDate, 
                   CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, RowVersion
            FROM [acc].[Account]
            WHERE AccountNumber = @AccountNumber;";

        return await connection.QuerySingleOrDefaultAsync<Account>(sql, new { AccountNumber = accountNumber });
    }

    public async Task<Account> CreateAsync(Account account, Guid userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        if (connection.State != ConnectionState.Open)
        {
            connection.Open();
        }

        using var transaction = connection.BeginTransaction();
        try
        {
            const string sqlAccount = @"
                INSERT INTO [acc].[Account] 
                    (CustomerId, AccountNumber, AccountType, Balance, Currency, Status, InterestRate, CreatedBy)
                OUTPUT INSERTED.AccountId, INSERTED.OpenedDate, INSERTED.CreatedDate
                VALUES 
                    (@CustomerId, @AccountNumber, @AccountType, @Balance, @Currency, @Status, @InterestRate, @CreatedBy);";

            var inserted = await connection.QuerySingleAsync<(Guid AccountId, DateTime OpenedDate, DateTime CreatedDate)>(
                sqlAccount, 
                new 
                { 
                    account.CustomerId, 
                    account.AccountNumber, 
                    account.AccountType, 
                    account.Balance, 
                    account.Currency, 
                    account.Status, 
                    account.InterestRate, 
                    CreatedBy = userId 
                }, 
                transaction);

            account.AccountId = inserted.AccountId;
            account.OpenedDate = inserted.OpenedDate;
            account.CreatedDate = inserted.CreatedDate;
            account.CreatedBy = userId;

            const string sqlLimit = @"
                INSERT INTO [acc].[AccountLimit] (AccountId)
                VALUES (@AccountId);";

            await connection.ExecuteAsync(sqlLimit, new { AccountId = account.AccountId }, transaction);

            transaction.Commit();
            return account;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<bool> UpdateStatusAsync(Guid accountId, string status, Guid userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE [acc].[Account]
            SET Status = @Status,
                UpdatedBy = @UpdatedBy,
                UpdatedDate = SYSUTCDATETIME(),
                ClosedDate = CASE WHEN @Status = 'Closed' THEN SYSUTCDATETIME() ELSE ClosedDate END
            WHERE AccountId = @AccountId;";

        var rowsAffected = await connection.ExecuteAsync(sql, new { AccountId = accountId, Status = status, UpdatedBy = userId });
        return rowsAffected > 0;
    }

    public async Task<AccountLimit?> GetLimitsAsync(Guid accountId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT LimitId, AccountId, DailyTransferLimit, DailyWithdrawalLimit, TransactionLimit, UpdatedDate
            FROM [acc].[AccountLimit]
            WHERE AccountId = @AccountId;";

        return await connection.QuerySingleOrDefaultAsync<AccountLimit>(sql, new { AccountId = accountId });
    }

    public async Task<bool> UpdateLimitsAsync(AccountLimit limits)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE [acc].[AccountLimit]
            SET DailyTransferLimit = @DailyTransferLimit,
                DailyWithdrawalLimit = @DailyWithdrawalLimit,
                TransactionLimit = @TransactionLimit,
                UpdatedDate = SYSUTCDATETIME()
            WHERE AccountId = @AccountId;";

        var rowsAffected = await connection.ExecuteAsync(sql, limits);
        return rowsAffected > 0;
    }
}

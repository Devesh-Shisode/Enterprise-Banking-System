using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using CustomerAPI.Api.Data;

namespace CustomerAPI.Api.Data;

public class CustomerRepository : ICustomerRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CustomerRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<bool> CreateCustomerAsync(Customer customer)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO [cust].[Customer] 
                (CustomerId, FirstName, LastName, DateOfBirth, Gender, PAN, AadhaarNumber, KYCStatus, KYCDate, CreatedDate)
            VALUES 
                (@CustomerId, @FirstName, @LastName, @DateOfBirth, @Gender, @PAN, @AadhaarNumber, @KYCStatus, @KYCDate, SYSUTCDATETIME())";

        var affected = await connection.ExecuteAsync(sql, customer);
        return affected > 0;
    }

    public async Task<Customer?> GetCustomerByIdAsync(Guid customerId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT CustomerId, FirstName, LastName, DateOfBirth, Gender, PAN, AadhaarNumber, KYCStatus, KYCDate, CreatedDate, UpdatedDate
            FROM [cust].[Customer]
            WHERE CustomerId = @CustomerId;

            SELECT AddressId, CustomerId, AddressType, AddressLine1, AddressLine2, City, State, PostalCode, Country, CreatedDate
            FROM [cust].[CustomerAddress]
            WHERE CustomerId = @CustomerId;

            SELECT ContactId, CustomerId, ContactType, ContactValue, IsPrimary, CreatedDate
            FROM [cust].[CustomerContact]
            WHERE CustomerId = @CustomerId;";

        using var multi = await connection.QueryMultipleAsync(sql, new { CustomerId = customerId });
        
        var customer = await multi.ReadSingleOrDefaultAsync<Customer>();
        if (customer != null)
        {
            customer.Addresses = (await multi.ReadAsync<CustomerAddress>()).ToList();
            customer.Contacts = (await multi.ReadAsync<CustomerContact>()).ToList();
        }

        return customer;
    }

    public async Task<bool> AddAddressAsync(CustomerAddress address)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO [cust].[CustomerAddress]
                (CustomerId, AddressType, AddressLine1, AddressLine2, City, State, PostalCode, Country, CreatedDate)
            VALUES
                (@CustomerId, @AddressType, @AddressLine1, @AddressLine2, @City, @State, @PostalCode, @Country, SYSUTCDATETIME())";

        var affected = await connection.ExecuteAsync(sql, address);
        return affected > 0;
    }

    public async Task<bool> AddContactAsync(CustomerContact contact)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO [cust].[CustomerContact]
                (CustomerId, ContactType, ContactValue, IsPrimary, CreatedDate)
            VALUES
                (@CustomerId, @ContactType, @ContactValue, @IsPrimary, SYSUTCDATETIME())";

        var affected = await connection.ExecuteAsync(sql, contact);
        return affected > 0;
    }

    public async Task<bool> UpdateKYCStatusAsync(Guid customerId, string pan, string aadhaar, string kycStatus)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE [cust].[Customer]
            SET PAN = @PAN,
                AadhaarNumber = @AadhaarNumber,
                KYCStatus = @KYCStatus,
                KYCDate = SYSUTCDATETIME(),
                UpdatedDate = SYSUTCDATETIME()
            WHERE CustomerId = @CustomerId";

        var affected = await connection.ExecuteAsync(sql, new 
        { 
            CustomerId = customerId, 
            PAN = pan, 
            AadhaarNumber = aadhaar, 
            KYCStatus = kycStatus 
        });

        return affected > 0;
    }
}

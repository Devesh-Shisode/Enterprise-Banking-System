using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using MasterAPI.Api.Configuration;

namespace MasterAPI.Api.Data;

public class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly DatabaseOptions _databaseOptions;

    public SqlConnectionFactory(IOptions<DatabaseOptions> databaseOptions)
    {
        _databaseOptions = databaseOptions.Value;
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_databaseOptions.DefaultConnection);
    }
}

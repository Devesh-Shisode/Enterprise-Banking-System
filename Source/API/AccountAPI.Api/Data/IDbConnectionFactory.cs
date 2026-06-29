using System.Data;

namespace AccountAPI.Api.Data;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}

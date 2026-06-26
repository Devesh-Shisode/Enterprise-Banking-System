using System.Data;

namespace IdentityAPI.Api.Data;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}

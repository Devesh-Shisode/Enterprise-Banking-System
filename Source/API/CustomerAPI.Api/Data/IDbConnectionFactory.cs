using System.Data;

namespace CustomerAPI.Api.Data;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}

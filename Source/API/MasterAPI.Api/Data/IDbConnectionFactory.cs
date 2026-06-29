using System.Data;

namespace MasterAPI.Api.Data;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}

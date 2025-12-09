using System.Data;

namespace PROC1_API.Repository
{
    public interface IDbConnectionFactory
    {

        IDbConnection CreateConnection();

    }
}

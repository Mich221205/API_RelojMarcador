using System.Data;

namespace MarcasInconsistencias.Repository
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}

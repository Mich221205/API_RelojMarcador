using System.Data;

namespace API_Autenticacion.Repository
{
    public interface IDbConnectionFactory
    {
        Task<IDbConnection> CreateConnectionAsync();
    }
}

using System.Data;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace API_Autenticacion.Repository
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public DbConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<IDbConnection> CreateConnectionAsync()
        {
            var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();
            return conn;
        }
    }
}

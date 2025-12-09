using MySql.Data.MySqlClient;
using System.Data;

namespace ADM16.Repository
{
    public class DbConnectionFactory : IDbConnectionFactory
    {


        private readonly IConfiguration _configuration;
        public DbConnectionFactory(IConfiguration configuration) => _configuration = configuration;

        public IDbConnection CreateConnection()
        {
            var cs = _configuration.GetConnectionString("DefaultConnection");
            return new MySqlConnection(cs);
        }
    }
}

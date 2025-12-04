using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;


namespace PROC1_API.Repository
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

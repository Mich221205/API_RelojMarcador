using System.Data;
using API_Autenticacion.Entities;
using Dapper;

namespace API_Autenticacion.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UsuarioRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Usuario?> ObtenerPorIdentificacionAsync(string identificacion)
        {
            using var conn = await _connectionFactory.CreateConnectionAsync();

            var sql = @"
                SELECT 
                    ID_Usuario,
                    ID_Tipo_Identificacion,
                    Identificacion,
                    Nombre,
                    Apellido_1,
                    Apellido_2,
                    Correo,
                    Telefono,
                    ID_Rol_Usuario,
                    Contrasena,
                    Fecha_Creacion,
                    Estado
                FROM Usuario
                WHERE Identificacion = @Identificacion;";

            return await conn.QuerySingleOrDefaultAsync<Usuario>(sql, new { Identificacion = identificacion });
        }

        public async Task BloquearUsuarioAsync(int idUsuario)
        {
            using var conn = await _connectionFactory.CreateConnectionAsync();

            var sql = @"UPDATE Usuario SET Estado = 0 WHERE ID_Usuario = @IdUsuario;";

            await conn.ExecuteAsync(sql, new { IdUsuario = idUsuario });
        }
    }
}

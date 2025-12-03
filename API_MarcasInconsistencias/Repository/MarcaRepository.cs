using Dapper;
using MarcasInconsistencias.Entities;
using System.Data;

namespace MarcasInconsistencias.Repository
{
    public class MarcaRepository
    {
        private readonly IDbConnectionFactory _db;

        public MarcaRepository(IDbConnectionFactory db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Marca>> GetByUsuarioAsync(int idUsuario)
        {
            using var conn = _db.CreateConnection();

            const string sql = @"
                SELECT 
                    ID_Marca,
                    ID_Usuario,
                    ID_Area,
                    Detalle,
                    CAST(Tipo_Marca AS CHAR(10)) AS Tipo_Marca,
                    Fecha_Hora,
                    IP_Usuario,
                    Latitud,
                    Longitud,
                    Ciudad,
                    Direccion,
                    Pais
                FROM marca
                WHERE ID_Usuario = @idUsuario
                ORDER BY Fecha_Hora DESC";

            return await conn.QueryAsync<Marca>(sql, new { idUsuario });
        }
    }
}

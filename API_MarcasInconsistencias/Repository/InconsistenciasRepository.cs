using Dapper;
using MarcasInconsistencias.Entities;
using System.Data;

namespace MarcasInconsistencias.Repository
{
    public class InconsistenciasRepository
    {
        private readonly IDbConnectionFactory _db;

        public InconsistenciasRepository(IDbConnectionFactory db)
        {
            _db = db;
        }

        public async Task<IEnumerable<InconsistenciaReporte>> GetAllAsync()
        {
            using var conn = _db.CreateConnection();

            const string sql = @"
                SELECT 
                    iu.ID_Inconsistencia,
                    u.ID_Usuario,
                    i.Nombre_Inconsistencia,
                    iu.Fecha_Inconsistencia,
                    iu.Estado,
                    iu.Detalle,
                    iu.Referencia
                FROM inconsistencias_usuario iu
                INNER JOIN inconsistencia i 
                    ON iu.ID_Inconsistencia = i.ID_Inconsistencia
                INNER JOIN usuario u 
                    ON iu.Identificacion = u.Identificacion
                ORDER BY iu.Fecha_Inconsistencia DESC;
            ";

            return await conn.QueryAsync<InconsistenciaReporte>(sql);
        }
    }
}

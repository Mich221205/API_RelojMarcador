using System.Data;
using Dapper;
using API_MarcasInconsistencias.Entities;
using MarcasInconsistencias.Repository;

namespace API_MarcasInconsistencias.Repository
{
    public class Usr5Repository : IUsr5Repository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public Usr5Repository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<InconsistenciaPendiente>> ObtenerInconsistenciasUsuarioAsync(string identificacion)
        {
            using var conn = _connectionFactory.CreateConnection();

            var sql = @"
SELECT 
    iu.ID_Inconsistencia_Usuario AS IdInconsistenciaUsuario,
    iu.Fecha_Inconsistencia       AS FechaInconsistencia,
    i.Nombre_Inconsistencia       AS NombreInconsistencia
FROM inconsistencias_usuario iu
JOIN inconsistencia i 
      ON i.ID_Inconsistencia = iu.ID_Inconsistencia
WHERE iu.Identificacion = @Identificacion
  AND iu.Estado IN ('No Justificada','Pendiente')
ORDER BY iu.Fecha_Inconsistencia DESC;";

            return await conn.QueryAsync<InconsistenciaPendiente>(sql, new { Identificacion = identificacion });
        }

        public async Task<IEnumerable<MotivoAusencia>> ObtenerMotivosAsync()
        {
            using var conn = _connectionFactory.CreateConnection();

            var sql = @"
SELECT 
    ID_Motivo     AS IdMotivo,
    Nombre_Motivo AS NombreMotivo
FROM motivos_ausencia
ORDER BY Nombre_Motivo;";

            return await conn.QueryAsync<MotivoAusencia>(sql);
        }

        public async Task<bool> InconsistenciaPerteneceAlUsuarioAsync(int idInconsistencia, string identificacion)
        {
            using var conn = _connectionFactory.CreateConnection();

            var sql = @"
SELECT COUNT(*)
FROM inconsistencias_usuario
WHERE ID_Inconsistencia_Usuario = @Id
  AND Identificacion = @Identificacion
  AND Estado IN ('No Justificada','Pendiente');";

            var count = await conn.ExecuteScalarAsync<int>(sql, new
            {
                Id = idInconsistencia,
                Identificacion = identificacion
            });

            return count > 0;
        }

        public async Task<bool> ExisteJustificacionAsync(int idInconsistencia)
        {
            using var conn = _connectionFactory.CreateConnection();

            var sql = @"
SELECT COUNT(*)
FROM justificaciones
WHERE ID_Inconsistencia_Usuario = @Id;";

            var count = await conn.ExecuteScalarAsync<int>(sql, new { Id = idInconsistencia });
            return count > 0;
        }

        public async Task CrearJustificacionAsync(JustificacionCrearDto dto, string? adjuntoUrl, string descripcionBitacora)
        {
            using var conn = _connectionFactory.CreateConnection();
            using var tx = conn.BeginTransaction();

            try
            {
                var sqlInsertJust = @"
INSERT INTO justificaciones
    (ID_Inconsistencia_Usuario, ID_Motivo, Descripcion, Adjunto_URL)
VALUES
    (@IdInc, @IdMotivo, @Desc, @Adjunto);";

                await conn.ExecuteAsync(
                    sqlInsertJust,
                    new
                    {
                        IdInc = dto.IdInconsistenciaUsuario,
                        IdMotivo = dto.IdMotivo,
                        Desc = dto.Descripcion,
                        Adjunto = adjuntoUrl
                    },
                    tx
                );

                var sqlUpdateInc = @"
UPDATE inconsistencias_usuario
SET Estado = 'Enviada'
WHERE ID_Inconsistencia_Usuario = @IdInc;";

                await conn.ExecuteAsync(
                    sqlUpdateInc,
                    new { IdInc = dto.IdInconsistenciaUsuario },
                    tx
                );

                var sqlBitacora = @"
INSERT INTO bitacora
    (Fecha_Registro, ID_Usuario, ID_Accion, Descripcion_Accion)
VALUES
    (CURDATE(), @IdUsuario, 4, @Descripcion);";

                await conn.ExecuteAsync(
                    sqlBitacora,
                    new
                    {
                        IdUsuario = dto.IdUsuario,
                        Descripcion = descripcionBitacora
                    },
                    tx
                );

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }
    }
}

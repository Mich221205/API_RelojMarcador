using System.Data;
using System.Text;
using Dapper;
using API_MarcasInconsistencias.Entities;
using MarcasInconsistencias.Repository;

namespace API_MarcasInconsistencias.Repository
{
    public class Usr6Repository : IUsr6Repository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public Usr6Repository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<MotivoAusencia>> ObtenerMotivosAsync()
        {
            using var conn = _connectionFactory.CreateConnection();

            const string sql = @"
SELECT 
    ID_Motivo     AS IdMotivo,
    Nombre_Motivo AS NombreMotivo
FROM motivos_ausencia
ORDER BY Nombre_Motivo;";

            return await conn.QueryAsync<MotivoAusencia>(sql);
        }

        public async Task<bool> TieneVacacionesSuperpuestasAsync(int idUsuario, DateTime fechaInicio, DateTime fechaFin)
        {
            using var conn = _connectionFactory.CreateConnection();

            const string sql = @"
SELECT COUNT(*)
FROM vacaciones
WHERE ID_Usuario = @IdUsuario
  AND Estado = 'Aprobado'
  AND NOT (Fecha_Fin < @FechaInicio OR Fecha_Inicio > @FechaFin);";

            var count = await conn.ExecuteScalarAsync<int>(sql, new
            {
                IdUsuario = idUsuario,
                FechaInicio = fechaInicio.Date,
                FechaFin = fechaFin.Date
            });

            return count > 0;
        }

        public async Task CrearPermisoAsync(PermisoCrearDto dto, string? adjuntoUrl, string descripcionBitacoraJson)
        {
            using var conn = _connectionFactory.CreateConnection();
            using var tx = conn.BeginTransaction();

            try
            {
                const string sqlInsert = @"
INSERT INTO permisos
    (ID_Usuario, Fecha_Inicio, Hora_Inicio, Fecha_Fin, Hora_Fin, ID_Motivo, Observaciones, Adjunto_URL, Estado)
VALUES
    (@IdUsuario, @FechaInicio, @HoraInicio, @FechaFin, @HoraFin, @IdMotivo, @Observaciones, @AdjuntoUrl, 'Pendiente');";

                await conn.ExecuteAsync(
                    sqlInsert,
                    new
                    {
                        dto.IdUsuario,
                        FechaInicio = dto.FechaHoraInicio.Date,
                        HoraInicio = dto.FechaHoraInicio.TimeOfDay,
                        FechaFin = dto.FechaHoraFin.Date,
                        HoraFin = dto.FechaHoraFin.TimeOfDay,
                        dto.IdMotivo,
                        Observaciones = dto.Observaciones,
                        AdjuntoUrl = adjuntoUrl
                    },
                    tx
                );

                const string sqlBitacora = @"
INSERT INTO bitacora
    (Fecha_Registro, ID_Usuario, ID_Accion, Descripcion_Accion)
VALUES
    (CURDATE(), @IdUsuario, 1, @Descripcion);";

                await conn.ExecuteAsync(
                    sqlBitacora,
                    new
                    {
                        dto.IdUsuario,
                        Descripcion = descripcionBitacoraJson
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

        public async Task<IEnumerable<PermisoListDto>> ObtenerPermisosAsync(PermisoFiltroDto filtro)
        {
            using var conn = _connectionFactory.CreateConnection();

            var sb = new StringBuilder();
            sb.Append(@"
SELECT 
    p.ID_Permiso      AS IdPermiso,
    p.Fecha_Inicio    AS FechaInicio,
    p.Hora_Inicio     AS HoraInicio,
    p.Fecha_Fin       AS FechaFin,
    p.Hora_Fin        AS HoraFin,
    p.ID_Motivo       AS IdMotivo,
    m.Nombre_Motivo   AS NombreMotivo,
    p.Observaciones   AS Observaciones,
    p.Adjunto_URL     AS AdjuntoUrl,
    p.Estado          AS Estado,
    p.Fecha_Solicitud AS FechaSolicitud
FROM permisos p
JOIN motivos_ausencia m ON m.ID_Motivo = p.ID_Motivo
WHERE p.ID_Usuario = @IdUsuario");

            var parameters = new DynamicParameters();
            parameters.Add("IdUsuario", filtro.IdUsuario);

            if (filtro.Estado is "Pendiente" or "Aprobado" or "Rechazado")
            {
                sb.Append(" AND p.Estado = @Estado");
                parameters.Add("Estado", filtro.Estado);
            }

            if (filtro.Desde.HasValue && filtro.Hasta.HasValue)
            {
                sb.Append(" AND NOT (p.Fecha_Fin < @Desde OR p.Fecha_Inicio > @Hasta)");
                parameters.Add("Desde", filtro.Desde.Value.Date);
                parameters.Add("Hasta", filtro.Hasta.Value.Date);
            }
            else if (filtro.Desde.HasValue)
            {
                sb.Append(" AND p.Fecha_Fin >= @Desde");
                parameters.Add("Desde", filtro.Desde.Value.Date);
            }
            else if (filtro.Hasta.HasValue)
            {
                sb.Append(" AND p.Fecha_Inicio <= @Hasta");
                parameters.Add("Hasta", filtro.Hasta.Value.Date);
            }

            sb.Append(" ORDER BY p.Fecha_Solicitud DESC;");

            var sql = sb.ToString();
            return await conn.QueryAsync<PermisoListDto>(sql, parameters);
        }
    }
}

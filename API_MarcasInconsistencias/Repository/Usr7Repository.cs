using System.Data;
using Dapper;
using API_MarcasInconsistencias.Entities;
using MarcasInconsistencias.Repository;

namespace API_MarcasInconsistencias.Repository
{
    public class Usr7Repository : IUsr7Repository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public Usr7Repository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // ============================
        // CREAR NUEVA VACACIÓN
        // ============================
        public async Task<bool> CrearVacacionAsync(VacacionNuevaDto dto)
        {
            using var conn = _connectionFactory.CreateConnection();

            var sql = @"
INSERT INTO solicitud
(ID_Solicitante, Tipo, Fecha_Inicio, Fecha_Fin, Motivo, Adjuntos, Decision)
VALUES
(@IdSolicitante, 'Vacaciones', @FechaInicio, @FechaFin, @Motivo, @Adjuntos, 'Pendiente')";

            var rows = await conn.ExecuteAsync(sql, dto);
            return rows > 0;
        }

        // ============================
        // VER MIS SOLICITUDES
        // ============================
        public async Task<(IEnumerable<Vacacion>, int)> ObtenerMisSolicitudesAsync(VacacionFiltro filtro)
        {
            using var conn = _connectionFactory.CreateConnection();

            var sqlBase = @"
FROM solicitud s
WHERE s.ID_Solicitante = @IdUsuario
  AND s.Tipo = 'Vacaciones'";

            var p = new DynamicParameters();
            p.Add("IdUsuario", filtro.IdUsuario);

            var sqlCount = $"SELECT COUNT(*) {sqlBase}";
            var total = await conn.ExecuteScalarAsync<int>(sqlCount, p);

            var offset = (filtro.Pagina - 1) * filtro.PageSize;

            var sql = $@"
SELECT 
    s.ID AS Id,
    s.Tipo,
    s.Fecha_Inicio AS FechaInicio,
    s.Fecha_Fin AS FechaFin,
    s.Decision AS Estado,
    s.Observacion,
    s.Fecha_Resolucion
{sqlBase}
ORDER BY s.ID DESC
LIMIT @PageSize OFFSET @Offset";

            p.Add("PageSize", filtro.PageSize);
            p.Add("Offset", offset);

            var datos = await conn.QueryAsync<Vacacion>(sql, p);

            return (datos, total);
        }

        // ============================
        // PROCESAR VACACIÓN
        // ============================
        public async Task<bool> ProcesarVacacionAsync(VacacionProcesarDto dto)
        {
            using var conn = _connectionFactory.CreateConnection();

            var sql = @"
UPDATE solicitud
SET 
    Decision = @Accion,
    Observacion = @Observacion,
    Fecha_Resolucion = NOW()
WHERE ID = @IdSolicitud";

            var rows = await conn.ExecuteAsync(sql, dto);
            return rows > 0;
        }
    }
}

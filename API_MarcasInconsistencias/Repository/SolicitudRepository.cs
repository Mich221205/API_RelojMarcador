using System.Data;
using Dapper;
using API_MarcasInconsistencias.Entities;
using MarcasInconsistencias.Repository;

namespace API_MarcasInconsistencias.Repository
{
    public class SolicitudRepository : ISolicitudRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public SolicitudRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<(IEnumerable<Solicitud>, int)> ObtenerSolicitudesAsync(SolicitudFiltro filtro)
        {
            using var connection = _connectionFactory.CreateConnection();

            var sqlBase = @"
FROM solicitud s
INNER JOIN usuario u ON s.ID_Solicitante = u.ID_Usuario
WHERE 1=1";

            var p = new DynamicParameters();

            if (!string.IsNullOrEmpty(filtro.Tipo))
            {
                sqlBase += " AND s.Tipo = @Tipo";
                p.Add("Tipo", filtro.Tipo);
            }

            if (!string.IsNullOrEmpty(filtro.Estado))
            {
                sqlBase += " AND s.Decision = @Estado";
                p.Add("Estado", filtro.Estado);
            }

            if (!string.IsNullOrEmpty(filtro.Funcionario))
            {
                sqlBase += @"
AND (u.Nombre LIKE CONCAT('%', @Func, '%') 
 OR u.Apellido_1 LIKE CONCAT('%', @Func, '%')
 OR u.Apellido_2 LIKE CONCAT('%', @Func, '%'))";
                p.Add("Func", filtro.Funcionario);
            }

            if (filtro.FechaInicio.HasValue)
            {
                sqlBase += " AND DATE(s.Fecha_Inicio) >= @F1";
                p.Add("F1", filtro.FechaInicio.Value.Date);
            }

            if (filtro.FechaFin.HasValue)
            {
                sqlBase += " AND DATE(s.Fecha_Fin) <= @F2";
                p.Add("F2", filtro.FechaFin.Value.Date);
            }

            var sqlCount = "SELECT COUNT(*) " + sqlBase;
            var total = await connection.ExecuteScalarAsync<int>(sqlCount, p);

            var offset = (filtro.Pagina - 1) * filtro.PageSize;

            var sql = $@"
SELECT
    s.ID AS Id,
    s.Tipo,
    CONCAT(u.Nombre, ' ', u.Apellido_1, ' ', u.Apellido_2) AS Solicitante,
    u.Identificacion,
    s.Fecha_Inicio AS FechaInicio,
    s.Fecha_Fin AS FechaFin,
    s.Decision,
    s.Observacion,
    s.Fecha_Resolucion
{sqlBase}
ORDER BY s.ID DESC
LIMIT @PageSize OFFSET @Offset";

            p.Add("PageSize", filtro.PageSize);
            p.Add("Offset", offset);

            var datos = await connection.QueryAsync<Solicitud>(sql, p);

            return (datos, total);
        }

        public async Task<SolicitudDetalle?> ObtenerDetalleAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"
SELECT
    s.ID,
    s.Tipo,
    CONCAT(u.Nombre, ' ', u.Apellido_1, ' ', u.Apellido_2) AS Solicitante,
    u.Identificacion,
    s.Fecha_Inicio AS FechaInicio,
    s.Fecha_Fin AS FechaFin,
    s.Decision AS Estado,
    s.Observacion,
    s.Fecha_Resolucion,
    s.Adjuntos
FROM solicitud s
INNER JOIN usuario u ON s.ID_Solicitante = u.ID_Usuario
WHERE s.ID = @Id";

            return await connection.QueryFirstOrDefaultAsync<SolicitudDetalle>(sql, new { Id = id });
        }

        public async Task<bool> ProcesarSolicitudAsync(SolicitudProcesarDto dto)
        {
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"
UPDATE solicitud
SET Decision = @Accion,
    Observacion = @Observacion,
    Fecha_Resolucion = NOW()
WHERE ID = @IdSolicitud";

            var rows = await connection.ExecuteAsync(sql, dto);
            return rows > 0;
        }
    }
}

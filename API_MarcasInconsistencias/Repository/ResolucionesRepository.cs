// Repository/ResolucionesRepository.cs
using System.Data;
using Dapper;
using API_MarcasInconsistencias.Entities;
using MarcasInconsistencias.Repository;

namespace API_MarcasInconsistencias.Repository
{
    public interface IResolucionesRepository
    {
        Task<(IEnumerable<ResolucionSolicitud> Datos, int TotalRegistros)>
            ObtenerResolucionesAsync(ResolucionesFiltro filtro);

        Task<IEnumerable<ResolucionSolicitud>> ExportarResolucionesAsync(ResolucionesFiltro filtro);
    }

    public class ResolucionesRepository : IResolucionesRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ResolucionesRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        private static (string sqlBase, DynamicParameters parametros)
            ConstruirSqlBase(ResolucionesFiltro filtro)
        {
            var sqlBase = @"
FROM solicitud s
INNER JOIN usuario u ON s.ID_Solicitante = u.ID_Usuario
WHERE s.Decision IN ('Aprobado', 'Rechazado')";

            var p = new DynamicParameters();

            // Tipo
            if (!string.IsNullOrWhiteSpace(filtro.Tipo) && filtro.Tipo != "Todas")
            {
                sqlBase += " AND s.Tipo = @Tipo";
                p.Add("Tipo", filtro.Tipo);
            }

            // Estado (Aprobado / Rechazado)
            if (!string.IsNullOrWhiteSpace(filtro.Estado))
            {
                sqlBase += " AND s.Decision = @Estado";
                p.Add("Estado", filtro.Estado);
            }

            // Funcionario (nombre, apellidos o identificación)
            if (!string.IsNullOrWhiteSpace(filtro.Funcionario))
            {
                sqlBase += @"
 AND (u.Nombre LIKE CONCAT('%', @Func, '%')
      OR u.Apellido_1 LIKE CONCAT('%', @Func, '%')
      OR u.Apellido_2 LIKE CONCAT('%', @Func, '%')
      OR u.Identificacion LIKE CONCAT('%', @Func, '%'))";
                p.Add("Func", filtro.Funcionario);
            }

            // Rango de fechas (Fecha_Resolucion)
            if (filtro.FechaInicio.HasValue && filtro.FechaFin.HasValue)
            {
                sqlBase += " AND DATE(s.Fecha_Resolucion) BETWEEN @FIni AND @FFin";
                p.Add("FIni", filtro.FechaInicio.Value.Date);
                p.Add("FFin", filtro.FechaFin.Value.Date);
            }
            else if (filtro.FechaInicio.HasValue)
            {
                sqlBase += " AND DATE(s.Fecha_Resolucion) >= @FIni";
                p.Add("FIni", filtro.FechaInicio.Value.Date);
            }
            else if (filtro.FechaFin.HasValue)
            {
                sqlBase += " AND DATE(s.Fecha_Resolucion) <= @FFin";
                p.Add("FFin", filtro.FechaFin.Value.Date);
            }

            return (sqlBase, p);
        }

        public async Task<(IEnumerable<ResolucionSolicitud> Datos, int TotalRegistros)>
            ObtenerResolucionesAsync(ResolucionesFiltro filtro)
        {
            if (filtro.Pagina < 1) filtro.Pagina = 1;
            if (filtro.RegistrosPorPagina <= 0) filtro.RegistrosPorPagina = 10;

            var (sqlBase, parametros) = ConstruirSqlBase(filtro);

            using var connection = _connectionFactory.CreateConnection();

            // Total registros (equivalente a $sql_count)
            var sqlCount = "SELECT COUNT(*) " + sqlBase;
            var total = await connection.ExecuteScalarAsync<int>(sqlCount, parametros);

            // Consulta principal con LIMIT y OFFSET
            var offset = (filtro.Pagina - 1) * filtro.RegistrosPorPagina;

            var sql = $@"
SELECT
    s.ID            AS Id,
    s.Tipo          AS Tipo,
    CONCAT(u.Nombre, ' ', u.Apellido_1, ' ', u.Apellido_2) AS Solicitante,
    u.Identificacion AS Identificacion,
    s.Fecha_Inicio  AS FechaInicio,
    s.Fecha_Fin     AS FechaFin,
    s.Decision      AS Decision,
    s.Fecha_Resolucion AS FechaResolucion,
    s.Observacion   AS Observacion
{sqlBase}
ORDER BY s.Fecha_Resolucion DESC
LIMIT @PageSize OFFSET @Offset;";

            parametros.Add("PageSize", filtro.RegistrosPorPagina);
            parametros.Add("Offset", offset);

            var datos = await connection.QueryAsync<ResolucionSolicitud>(sql, parametros);

            return (datos, total);
        }

        public async Task<IEnumerable<ResolucionSolicitud>> ExportarResolucionesAsync(ResolucionesFiltro filtro)
        {
            var (sqlBase, parametros) = ConstruirSqlBase(filtro);

            using var connection = _connectionFactory.CreateConnection();

            var sql = $@"
SELECT
    s.ID            AS Id,
    s.Tipo          AS Tipo,
    CONCAT(u.Nombre, ' ', u.Apellido_1, ' ', u.Apellido_2) AS Solicitante,
    u.Identificacion AS Identificacion,
    s.Fecha_Inicio  AS FechaInicio,
    s.Fecha_Fin     AS FechaFin,
    s.Decision      AS Decision,
    s.Fecha_Resolucion AS FechaResolucion,
    s.Observacion   AS Observacion
{sqlBase}
ORDER BY s.Fecha_Resolucion DESC;";

            var datos = await connection.QueryAsync<ResolucionSolicitud>(sql, parametros);
            return datos;
        }
    }
}

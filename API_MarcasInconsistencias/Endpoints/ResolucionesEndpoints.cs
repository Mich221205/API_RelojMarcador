// Endpoints/ResolucionesEndpoints.cs
using System.Globalization;
using System.Text;
using API_MarcasInconsistencias.Entities;
using API_MarcasInconsistencias.Services;

namespace API_MarcasInconsistencias.Endpoints
{
    public static class ResolucionesEndpoints
    {
        public static IEndpointRouteBuilder MapResolucionesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/resoluciones")
                           .WithTags("Resoluciones");

            // GET /api/resoluciones?tipo=&estado=&funcionario=&fechaInicio=&fechaFin=&pagina=&pageSize=
            group.MapGet("/", async (
                [AsParameters] ResolucionesQuery query,
                IResolucionesService service) =>
            {
                var filtro = query.ToFiltro();
                var resultado = await service.ObtenerResolucionesAsync(filtro);
                return Results.Ok(resultado);
            });

            // GET /api/resoluciones/export (devuelve CSV)
            group.MapGet("/export", async (
                [AsParameters] ResolucionesQuery query,
                IResolucionesService service) =>
            {
                var filtro = query.ToFiltro();
                var datos = await service.ExportarResolucionesAsync(filtro);

                var sb = new StringBuilder();
                sb.AppendLine("ID,Tipo,Solicitante,Identificacion,FechaInicio,FechaFin,Decision,FechaResolucion,Observacion");

                foreach (var r in datos)
                {
                    string CsvSafe(string? v) =>
                        string.IsNullOrEmpty(v)
                            ? ""
                            : "\"" + v.Replace("\"", "\"\"") + "\"";

                    sb.AppendLine(string.Join(",", new[]
                    {
                        r.Id.ToString(),
                        CsvSafe(r.Tipo),
                        CsvSafe(r.Solicitante),
                        CsvSafe(r.Identificacion),
                        r.FechaInicio?.ToString("yyyy-MM-dd") ?? "",
                        r.FechaFin?.ToString("yyyy-MM-dd") ?? "",
                        CsvSafe(r.Decision),
                        r.FechaResolucion?.ToString("yyyy-MM-dd HH:mm:ss") ?? "",
                        CsvSafe(r.Observacion)
                    }));
                }

                var bytes = Encoding.UTF8.GetBytes(sb.ToString());
                return Results.File(bytes, "text/csv; charset=utf-8", "resoluciones_export.csv");
            });

            return app;
        }

        // Clase que mapea los query params (equivalente a $_GET)
        public class ResolucionesQuery
        {
            public string? Tipo { get; set; } = "Todas";
            public string? Estado { get; set; }
            public string? Funcionario { get; set; }
            public string? FechaInicio { get; set; }
            public string? FechaFin { get; set; }

            public int? Pagina { get; set; }
            public int? PageSize { get; set; }

            public ResolucionesFiltro ToFiltro()
            {
                return new ResolucionesFiltro
                {
                    Tipo = Tipo,
                    Estado = Estado,
                    Funcionario = Funcionario,
                    FechaInicio = ParseDate(FechaInicio),
                    FechaFin = ParseDate(FechaFin),

                    // 👇 Valores por defecto si vienen nulos
                    Pagina = Pagina ?? 1,
                    RegistrosPorPagina = PageSize ?? 10
                };
            }

            private DateTime? ParseDate(string? s)
            {
                if (string.IsNullOrWhiteSpace(s)) return null;
                if (DateTime.TryParse(s, out var d)) return d;
                return null;
            }
        }

    }
}

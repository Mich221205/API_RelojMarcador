using API_MarcasInconsistencias.Entities;
using API_MarcasInconsistencias.Services;

namespace API_MarcasInconsistencias.Endpoints
{
    public static class SolicitudEndpoints
    {
        public static IEndpointRouteBuilder MapSolicitudEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/solicitud")
                           .WithTags("Solicitud");

            // LISTAR SOLICITUDES
            group.MapGet("/", async (
                [AsParameters] SolicitudQuery query,
                ISolicitudService service) =>
            {
                var filtro = query.ToFiltro();
                var resultado = await service.ObtenerSolicitudesAsync(filtro);

                return Results.Ok(resultado);
            });

            // DETALLE
            group.MapGet("/{id:int}", async (
                int id,
                ISolicitudService service) =>
            {
                var detalle = await service.ObtenerDetalleAsync(id);

                return detalle is null
                    ? Results.NotFound()
                    : Results.Ok(detalle);
            });

            // PROCESAR SOLICITUD
            group.MapPost("/procesar", async (
                SolicitudProcesarDto dto,
                ISolicitudService service) =>
            {
                var ok = await service.ProcesarSolicitudAsync(dto);

                return ok
                    ? Results.Ok(new { status = "success" })
                    : Results.BadRequest(new { error = "No se pudo procesar la solicitud" });
            });

            return app;
        }

        // QUERIES
        public class SolicitudQuery
        {
            public string? Tipo { get; set; }
            public string? Estado { get; set; }
            public string? Funcionario { get; set; }

            public string? FechaInicio { get; set; }
            public string? FechaFin { get; set; }

            public int? Pagina { get; set; }
            public int? PageSize { get; set; }

            public SolicitudFiltro ToFiltro()
            {
                return new SolicitudFiltro
                {
                    Tipo = Tipo,
                    Estado = Estado,
                    Funcionario = Funcionario,
                    FechaInicio = ParseDate(FechaInicio),
                    FechaFin = ParseDate(FechaFin),
                    Pagina = Pagina ?? 1,
                    PageSize = PageSize ?? 10
                };
            }

            private DateTime? ParseDate(string? s)
            {
                if (DateTime.TryParse(s, out var d))
                    return d;
                return null;
            }
        }
    }
}

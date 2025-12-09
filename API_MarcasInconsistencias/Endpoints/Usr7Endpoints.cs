using API_MarcasInconsistencias.Entities;
using API_MarcasInconsistencias.Services;

namespace API_MarcasInconsistencias.Endpoints
{
    public static class Usr7Endpoints
    {
        public static IEndpointRouteBuilder MapUsr7Endpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/usr7")
                           .WithTags("USR7 - Vacaciones");

            // ======================
            // NUEVA VACACIÓN
            // ======================
            group.MapPost("/vacaciones/nueva", async (
                VacacionNuevaDto dto,
                IUsr7Service service) =>
            {
                var ok = await service.CrearVacacionAsync(dto);

                return ok
                    ? Results.Ok(new { status = "success" })
                    : Results.BadRequest(new { error = "No se pudo crear la solicitud" });
            });

            // ======================
            // VER MIS SOLICITUDES
            // ======================
            group.MapGet("/mis-solicitudes/{idUsuario:int}", async (
                int idUsuario,
                int? pagina,
                int? pageSize,
                IUsr7Service service) =>
            {
                var filtro = new VacacionFiltro
                {
                    IdUsuario = idUsuario,
                    Pagina = pagina ?? 1,
                    PageSize = pageSize ?? 10
                };

                var data = await service.ObtenerMisSolicitudesAsync(filtro);
                return Results.Ok(data);
            });

            // ======================
            // PROCESAR VACACIÓN
            // ======================
            group.MapPost("/vacaciones/procesar", async (
                VacacionProcesarDto dto,
                IUsr7Service service) =>
            {
                var ok = await service.ProcesarVacacionAsync(dto);

                return ok
                    ? Results.Ok(new { status = "success" })
                    : Results.BadRequest(new { error = "No se pudo procesar" });
            });

            return app;
        }
    }
}

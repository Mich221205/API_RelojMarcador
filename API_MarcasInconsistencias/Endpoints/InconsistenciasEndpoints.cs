using Microsoft.AspNetCore.Mvc;
using MarcasInconsistencias.Services;

namespace MarcasInconsistencias.Endpoints
{
    public static class InconsistenciasEndpoints
    {
        public static void MapInconsistenciasEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/inconsistencias")
                              .WithTags("Inconsistencias")
                              .AllowAnonymous();

            // GET /inconsistencias?idUsuarioAccion=14
            group.MapGet("/", async (
                    [FromQuery] int idUsuarioAccion,
                    [FromServices] IInconsistenciasService service) =>
            {
                try
                {
                    var data = await service.GetAllAsync(idUsuarioAccion);
                    return Results.Ok(data);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("🔥 ERROR en endpoint /inconsistencias");
                    Console.WriteLine(ex.ToString());

                    return Results.BadRequest(new
                    {
                        error = ex.Message,
                        detalle = ex.ToString()
                    });
                }
            });
        }
    }
}

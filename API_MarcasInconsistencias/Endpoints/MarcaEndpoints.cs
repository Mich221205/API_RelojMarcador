using Microsoft.AspNetCore.Mvc;
using MarcasInconsistencias.Services;

namespace MarcasInconsistencias.Endpoints
{
    public static class MarcaEndpoints
    {
        public static void MapMarcaEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/marcas")
                              .WithTags("Marcas")
                              .AllowAnonymous();

            group.MapGet("/{idUsuario:int}", async (
                    int idUsuario,
                    [FromQuery] int? idUsuarioAccion,
                    HttpContext http,
                    [FromServices] IMarcaService service) =>
            {
                Console.WriteLine("==============");
                Console.WriteLine("PETICIÓN ENTRANTE:");
                Console.WriteLine("URL: " + http.Request.Path + http.Request.QueryString);
                Console.WriteLine("idUsuario: " + idUsuario);
                Console.WriteLine("idUsuarioAccion RAW: " + http.Request.Query["idUsuarioAccion"]);
                Console.WriteLine("==============");

                try
                {
                    if (!idUsuarioAccion.HasValue)
                        return Results.BadRequest(new { error = "Debe enviar idUsuarioAccion" });

                    var data = await service.GetByUsuarioAsync(idUsuario, idUsuarioAccion.Value);
                    return Results.Ok(data);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR EN ENDPOINT:");
                    Console.WriteLine(ex.ToString());
                    return Results.BadRequest(new { error = ex.Message, detalle = ex.ToString() });
                }
            });

        }
    }
}

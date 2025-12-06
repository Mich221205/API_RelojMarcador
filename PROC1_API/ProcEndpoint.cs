using Microsoft.AspNetCore.Mvc;
using PROC1_API.Entities;
using PROC1_API.Services;

namespace PROC1_API
{
    public static class PROC1Endpoints
    {
        public static void MapPROC1Endpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/proc1").WithTags("PROC1 – Inconsistencias");

            // ---------------------------------------------------------
            // Ejecutar proceso PROC1
            // ---------------------------------------------------------
            group.MapPost("/generar", async (
                [FromBody] EjecutarPROC1Request request,
                [FromServices] IProceso_Generar_Inconsistencias_MarcasService service
            ) =>
            {
                var response = await service.EjecutarProcesoAsync(
                    request.FechaInicio,
                    request.FechaFin,
                    request.AreaId,
                    request.UsuarioId
                );

                return Results.Json(response, statusCode: response.StatusCode);
            })
            .WithName("EjecutarProcesoInconsistencias")
            .WithOpenApi();


        }
    }
}

using ADM16.Entities;
using ADM16.Services;
using Microsoft.AspNetCore.Mvc;

namespace ADM16
{
    public static class ReporteMarcasEndpoint
    {
        public static void MapReportMarcasEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/reportmarcas")
                              .WithTags("Reporte de Marcas");

            // GET /api/marcas/reporte?page=1&pageSize=20&identificacion=&fecha=
            group.MapGet("/reporte", async (
                [FromQuery] int page,
                [FromQuery] int pageSize,
                [FromQuery] string? identificacion,
                [FromQuery] DateTime? fecha,
                [FromServices] IMarcaService service
            ) =>
            {
                var total = await service.Contar_Reporte_Marca(identificacion, fecha);
                var data = await service.Reporte_Marcas(page, pageSize, identificacion, fecha);

                var response = new ReporteMarcasResponse
                {
                    TotalRegistros = total,
                    Pagina = page,
                    PorPagina = pageSize,
                    Data = data
                };

                return Results.Ok(response);
            })
            .WithName("ReporteMarcas")
            .WithOpenApi();
        }
    }
}

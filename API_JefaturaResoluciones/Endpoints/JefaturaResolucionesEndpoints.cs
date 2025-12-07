using System.Text;
using API_JefaturaResoluciones.Repository;
using API_JefaturaResoluciones.Entities;

namespace API_JefaturaResoluciones.Endpoints;

public static class JefaturaResolucionesEndpoints
{
    public static void MapJefaturaResolucionesEndpoints(this WebApplication app)
    {
        app.MapGet("/api/jefatura/resoluciones", async (
            string? tipo,
            string? estado,
            int? idUsuario,
            DateTime? desde,
            DateTime? hasta,
            int page,
            int pageSize,
            JefaturaRepository repo) =>
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0 || pageSize > 50) pageSize = 10;

            var (items, total) = await repo.GetResolucionesAsync(
                tipo,
                estado,
                idUsuario,
                desde,
                hasta,
                page,
                pageSize);

            return Results.Ok(new
            {
                page,
                pageSize,
                total,
                items
            });
        });

        app.MapGet("/api/jefatura/resoluciones/csv", async (
            string? tipo,
            string? estado,
            int? idUsuario,
            DateTime? desde,
            DateTime? hasta,
            JefaturaRepository repo) =>
        {
            var (items, _) = await repo.GetResolucionesAsync(
                tipo,
                estado,
                idUsuario,
                desde,
                hasta,
                page: 1,
                pageSize: 10_000);

            var sb = new StringBuilder();
            sb.AppendLine("Tipo;IdRegistro;Funcionario;FechaDesde;FechaHasta;Decision;FechaResolucion;Observacion");

            foreach (var r in items)
            {
                sb.AppendLine(
                    $"{r.Tipo};{r.IdRegistro};{r.Funcionario};" +
                    $"{r.FechaDesde:yyyy-MM-dd HH:mm};{r.FechaHasta:yyyy-MM-dd HH:mm};" +
                    $"{r.Decision};{r.FechaResolucion:yyyy-MM-dd HH:mm};" +
                    $"\"{(r.Observacion ?? string.Empty).Replace("\"", "\"\"")}\"");
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return Results.File(bytes, "text/csv", "resoluciones.csv");
        });
    }
}

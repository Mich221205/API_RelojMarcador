using API_JefaturaResoluciones.Entities;
using API_JefaturaResoluciones.Repository;

namespace API_JefaturaResoluciones.Endpoints;

public static class JefaturaPendientesEndpoints
{
    public static void MapJefaturaPendientesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/jefatura");

        group.MapGet("/pendientes/permisos", async (
            int? idUsuario,
            DateTime? desde,
            DateTime? hasta,
            string? estado,
            bool? soloVacaciones,
            JefaturaRepository repo) =>
        {
            var items = await repo.GetPermisosPendientesAsync(
                idUsuario,
                desde,
                hasta,
                estado,
                soloVacaciones ?? false);

            return Results.Ok(items);
        });

        group.MapGet("/pendientes/justificaciones", async (
            int? idUsuario,
            DateTime? desde,
            DateTime? hasta,
            string? estado,
            JefaturaRepository repo) =>
        {
            var items = await repo.GetJustificacionesPendientesAsync(
                idUsuario,
                desde,
                hasta,
                estado);

            return Results.Ok(items);
        });

        // ========= RESOLVER PERMISO / VACACIONES =========

        group.MapPost("/permisos/resolver", async (
            ResolucionRequest request,
            JefaturaRepository repo) =>
        {
            if (string.IsNullOrWhiteSpace(request.Decision) ||
                string.IsNullOrWhiteSpace(request.Observacion) ||
                request.Observacion.Length < 10)
            {
                return Results.BadRequest(
                    "La decisión y la observación (mínimo 10 caracteres) son obligatorias.");
            }

            await repo.ResolverPermisoAsync(
                request.IdRegistro,
                request.Decision,
                request.Observacion,
                request.IdJefatura);

            return Results.Ok(new { message = "Permiso resuelto correctamente." });
        });


        group.MapPost("/justificaciones/resolver", async (
            ResolucionRequest request,
            JefaturaRepository repo) =>
        {
            if (string.IsNullOrWhiteSpace(request.Decision) ||
                string.IsNullOrWhiteSpace(request.Observacion) ||
                request.Observacion.Length < 10)
            {
                return Results.BadRequest(
                    "La decisión y la observación (mínimo 10 caracteres) son obligatorias.");
            }

            await repo.ResolverJustificacionAsync(
                request.IdRegistro,
                request.Decision,
                request.Observacion,
                request.IdJefatura);

            return Results.Ok(new { message = "Justificación resuelta correctamente." });
        });
    }
}

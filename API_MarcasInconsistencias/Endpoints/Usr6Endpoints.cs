using API_MarcasInconsistencias.Entities;
using API_MarcasInconsistencias.Services;
using Microsoft.AspNetCore.Mvc;

namespace API_MarcasInconsistencias.Endpoints
{
    public static class Usr6Endpoints
    {
        public static IEndpointRouteBuilder MapUsr6Endpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/usr6")
                           .WithTags("USR6 - Permisos");

            // GET /api/usr6/motivos
            group.MapGet("/motivos", async (IUsr6Service service) =>
            {
                var motivos = await service.ObtenerMotivosAsync();
                return Results.Ok(motivos);
            });

            // GET /api/usr6/permisos?idUsuario=...&estado=...&desde=...&hasta=...
            group.MapGet("/permisos", async (
                [FromQuery] int idUsuario,
                [FromQuery] string? estado,
                [FromQuery] DateTime? desde,
                [FromQuery] DateTime? hasta,
                IUsr6Service service) =>
            {
                if (idUsuario <= 0)
                    return Results.BadRequest(new { error = "IdUsuario requerido." });

                var filtro = new PermisoFiltroDto
                {
                    IdUsuario = idUsuario,
                    Estado = string.IsNullOrWhiteSpace(estado) ? "Todos" : estado,
                    Desde = desde,
                    Hasta = hasta
                };

                var permisos = await service.ObtenerPermisosAsync(filtro);
                return Results.Ok(permisos);
            });

            // POST /api/usr6/permiso   (multipart/form-data)
            group.MapPost("/permiso", async (
                [FromForm] PermisoForm form,
                IUsr6Service service,
                IWebHostEnvironment env) =>
            {
                var errores = new List<string>();
                string? adjuntoUrl = null;

                // Manejo del archivo adjunto
                if (form.Adjunto is not null && form.Adjunto.Length > 0)
                {
                    const long maxBytes = 5L * 1024L * 1024L;
                    if (form.Adjunto.Length > maxBytes)
                    {
                        errores.Add("El archivo no debe superar 5 MB.");
                    }
                    else
                    {
                        var permitidas = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
                        var ext = Path.GetExtension(form.Adjunto.FileName).ToLowerInvariant();

                        if (!permitidas.Contains(ext))
                        {
                            errores.Add("Formato no permitido. Solo PDF, JPG, JPEG, PNG.");
                        }
                        else
                        {
                            var webRoot = env.WebRootPath;
                            if (string.IsNullOrEmpty(webRoot))
                                webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                            var destDir = Path.Combine(webRoot, "uploads", "permisos");
                            if (!Directory.Exists(destDir))
                                Directory.CreateDirectory(destDir);

                            var nombre = $"P_{form.IdUsuario}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}{ext}";
                            var destFs = Path.Combine(destDir, nombre);

                            using (var stream = new FileStream(destFs, FileMode.Create))
                            {
                                await form.Adjunto.CopyToAsync(stream);
                            }

                            adjuntoUrl = Path.Combine("uploads", "permisos", nombre)
                                             .Replace("\\", "/");
                        }
                    }
                }

                if (errores.Count > 0)
                    return Results.BadRequest(new { errores });

                if (!DateTime.TryParse($"{form.FechaInicio} {form.HoraInicio}", out var ini))
                {
                    errores.Add("Formato de fecha/hora de inicio inválido.");
                }
                if (!DateTime.TryParse($"{form.FechaFin} {form.HoraFin}", out var fin))
                {
                    errores.Add("Formato de fecha/hora de fin inválido.");
                }

                if (errores.Count > 0)
                    return Results.BadRequest(new { errores });

                var dto = new PermisoCrearDto
                {
                    IdUsuario = form.IdUsuario,
                    Identificacion = form.Identificacion ?? string.Empty,
                    FechaHoraInicio = ini,
                    FechaHoraFin = fin,
                    IdMotivo = form.IdMotivo,
                    Observaciones = form.Observaciones
                };

                var (ok, erroresServicio) = await service.CrearPermisoAsync(dto, adjuntoUrl);

                if (!ok)
                    return Results.BadRequest(new { errores = erroresServicio });

                return Results.Ok(new { mensaje = "Permiso enviado correctamente. Estado inicial: Pendiente." });
            });

            return app;
        }

        public class PermisoForm
        {
            public int IdUsuario { get; set; }
            public string? Identificacion { get; set; }

            public string FechaInicio { get; set; } = string.Empty; // "YYYY-MM-DD"
            public string HoraInicio { get; set; } = string.Empty;  // "HH:mm"
            public string FechaFin { get; set; } = string.Empty;
            public string HoraFin { get; set; } = string.Empty;

            public int IdMotivo { get; set; }
            public string? Observaciones { get; set; }

            public IFormFile? Adjunto { get; set; }
        }
    }
}

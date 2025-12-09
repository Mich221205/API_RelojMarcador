using API_MarcasInconsistencias.Entities;
using API_MarcasInconsistencias.Services;
using Microsoft.AspNetCore.Mvc;

namespace API_MarcasInconsistencias.Endpoints
{
    public static class Usr5Endpoints
    {
        public static IEndpointRouteBuilder MapUsr5Endpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/usr5")
                           .WithTags("USR5 - Justificación de inconsistencias");

            // ======================
            // GET: Inconsistencias pendientes del usuario
            // /api/usr5/inconsistencias?identificacion=XXXX
            // ======================
            group.MapGet("/inconsistencias", async (
                [FromQuery] string identificacion,
                IUsr5Service service) =>
            {
                if (string.IsNullOrWhiteSpace(identificacion))
                    return Results.BadRequest(new { error = "Identificación requerida." });

                var incs = await service.ObtenerInconsistenciasAsync(identificacion);
                return Results.Ok(incs);
            });

            // ======================
            // GET: Motivos de ausencia
            // /api/usr5/motivos
            // ======================
            group.MapGet("/motivos", async (IUsr5Service service) =>
            {
                var motivos = await service.ObtenerMotivosAsync();
                return Results.Ok(motivos);
            });

            // ======================
            // POST: Crear justificación
            // /api/usr5/justificar
            // Recibe multipart/form-data (con archivo opcional)
            // ======================
            group.MapPost("/justificar", async (
                [FromForm] JustificacionForm form,
                IUsr5Service service,
                IWebHostEnvironment env) =>
            {
                var errores = new List<string>();
                string? adjuntoUrl = null;

                // --- Validación y guardado del archivo (como en PHP) ---
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

                            var destDir = Path.Combine(webRoot, "uploads", "justificaciones");
                            if (!Directory.Exists(destDir))
                            {
                                Directory.CreateDirectory(destDir);
                            }

                            var nombre = $"J_{form.Identificacion}_{form.IdInconsistenciaUsuario}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}{ext}";
                            var destFs = Path.Combine(destDir, nombre);

                            using (var stream = new FileStream(destFs, FileMode.Create))
                            {
                                await form.Adjunto.CopyToAsync(stream);
                            }

                            // Ruta relativa que se guarda en BD
                            adjuntoUrl = Path.Combine("uploads", "justificaciones", nombre)
                                               .Replace("\\", "/");
                        }
                    }
                }

                if (errores.Count > 0)
                {
                    return Results.BadRequest(new { errores });
                }

                var dto = new JustificacionCrearDto
                {
                    IdInconsistenciaUsuario = form.IdInconsistenciaUsuario,
                    IdMotivo = form.IdMotivo,
                    Descripcion = form.Descripcion ?? string.Empty,
                    IdUsuario = form.IdUsuario,
                    Identificacion = form.Identificacion ?? string.Empty
                };

                var (ok, erroresServicio) = await service.CrearJustificacionAsync(dto, adjuntoUrl);

                if (!ok)
                {
                    return Results.BadRequest(new { errores = erroresServicio });
                }

                return Results.Ok(new { mensaje = "Justificación enviada correctamente." });
            });

            return app;
        }

        // Clase auxiliar para binding de formulario multipart
        public class JustificacionForm
        {
            public int IdInconsistenciaUsuario { get; set; }
            public int IdMotivo { get; set; }
            public string? Descripcion { get; set; }

            public int IdUsuario { get; set; }
            public string? Identificacion { get; set; }

            public IFormFile? Adjunto { get; set; }
        }
    }
}

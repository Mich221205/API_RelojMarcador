using API_Autenticacion.Entities;
using API_Autenticacion.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace API_Autenticacion.Endpoints
{
    public static class AutenticacionEndpoints
    {
        // respuesta solo con el mensaje final
        public sealed record LoginOkResponse(string Mensaje);

        public static void MapAutenticacionEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/auth").WithTags("Autenticación");

            // POST /api/auth/login
            group.MapPost("/login",
                async (
                    [FromHeader(Name = "usuario")] string? usuario,
                    [FromHeader(Name = "contrasenna")] string? contrasenna,
                    IAutenticacionService service) =>
                {
                    // 400: headers faltantes
                    if (string.IsNullOrWhiteSpace(usuario) ||
                        string.IsNullOrWhiteSpace(contrasenna))
                    {
                        return Results.BadRequest(new
                        {
                            mensaje = "Headers 'usuario' y 'contrasenna' son obligatorios."
                        });
                    }

                    var resultado = await service.LoginAsync(usuario, contrasenna);

                    if (resultado.Estado == LoginEstado.Exitoso &&
                        resultado.Usuario is Usuario uOk)
                    {
                        var nombreCompleto = $"{uOk.Nombre} {uOk.Apellido_1} {uOk.Apellido_2}".Trim();
                        var mensaje = $"Bienvenido, {nombreCompleto}";

                        return Results.Ok(new LoginOkResponse(mensaje));
                    }

                    if (resultado.Estado == LoginEstado.CredencialesInvalidas)
                    {
                        return Results.Json(
                            new { mensaje = resultado.Mensaje },
                            statusCode: StatusCodes.Status401Unauthorized
                        );
                    }

                    if (resultado.Estado == LoginEstado.UsuarioBloqueado)
                    {
                        return Results.Json(
                            new { mensaje = resultado.Mensaje },
                            statusCode: StatusCodes.Status403Forbidden
                        );
                    }

                    return Results.Problem(resultado.Mensaje);
                })
                .Produces<LoginOkResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces(StatusCodes.Status500InternalServerError);
        }
    }
}

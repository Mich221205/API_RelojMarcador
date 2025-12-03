using MarcasInconsistencias.Endpoints;
using MarcasInconsistencias.Repository;
using MarcasInconsistencias.Services;
using Microsoft.OpenApi;

namespace MarcasInconsistencias
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ========= SWAGGER (Swashbuckle) =========
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "API Marcas",
                    Version = "v1"
                });
            });

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.SetMinimumLevel(LogLevel.Debug);

            // ========= CONTROLLERS =========
            builder.Services.AddControllers();

            // ========= BD (Dapper + MySQL) =========
            builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

            // ========= INYECCIÓN =========
            builder.Services.AddScoped<MarcaRepository>();
            builder.Services.AddScoped<IMarcaService, MarcaService>();
            builder.Services.AddScoped<BitacoraRepository>();

            builder.Services.AddScoped<InconsistenciasRepository>();
            builder.Services.AddScoped<IInconsistenciasService, InconsistenciasService>();


            var app = builder.Build();

            // ========= SWAGGER UI =========
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
                    c.RoutePrefix = "swagger";
                });
            }

            app.Use(async (context, next) =>
            {
                try
                {
                    await next.Invoke();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("EXCEPCIÓN GLOBAL:");
                    Console.WriteLine(ex.ToString());

                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        error = ex.Message,
                        detalle = ex.ToString()
                    });
                }
            });


            // ========= PIPELINE =========
            app.UseAuthorization();

            app.MapControllers();
            app.MapMarcaEndpoints();
            app.MapInconsistenciasEndpoints();

            app.Run();
        }
    }
}

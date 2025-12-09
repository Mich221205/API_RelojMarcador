using API_MarcasInconsistencias.Repository;
using API_MarcasInconsistencias.Services;
using API_MarcasInconsistencias.Endpoints;

using MarcasInconsistencias.Endpoints;
using MarcasInconsistencias.Repository;
using MarcasInconsistencias.Services;

using Microsoft.OpenApi;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;



namespace MarcasInconsistencias
{
    public class Program
    {
        public static void Main(string[] args)
        {

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            });



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

            // 
            builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

            builder.Services.AddScoped<IUsr5Repository, Usr5Repository>();
            builder.Services.AddScoped<IUsr5Service, Usr5Service>();

            builder.Services.AddScoped<IUsr6Repository, Usr6Repository>();
            builder.Services.AddScoped<IUsr6Service, Usr6Service>();

            builder.Services.AddScoped<IUsr7Repository, Usr7Repository>();
            builder.Services.AddScoped<IUsr7Service, Usr7Service>();

            



            var app = builder.Build();


            app.UseSwagger();
            app.UseSwaggerUI();

            // ========= SWAGGER UI =========
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI(c =>
            //    {
            //        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
            //        c.RoutePrefix = "swagger";
            //    });
            //}

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

            // 
            app.MapUsr5Endpoints();
            app.MapUsr6Endpoints();
            app.MapUsr7Endpoints();



            app.Run();
        }
    }
}

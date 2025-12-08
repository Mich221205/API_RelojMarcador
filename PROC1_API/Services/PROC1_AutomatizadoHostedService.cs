using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PROC1_API.Services
{
    public class PROC1_AutomatizadoHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public PROC1_AutomatizadoHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        //--------------------------------------------------------------
        // PROC1 GENERAR INCONSISTENCIAS DE MARCAS (Jocsan Fonseca)
        //--------------------------------------------------------------
        // Ejecuta automáticamente el proceso a horas específicas:
        // 12:20 PM, 5:20 PM, 10:40 PM
        //--------------------------------------------------------------

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var ahora = DateTime.Now;
                var horaActual = ahora.ToString("HH:mm");

               
                if (horaActual == "12:20" ||
                    horaActual == "17:20" ||
                    horaActual == "22:40")
                {
                    using var scope = _serviceProvider.CreateScope();

                    
                    var service = scope.ServiceProvider
                        .GetRequiredService<IProceso_Generar_Inconsistencias_MarcasService>();

                    try
                    {
                        
                        await service.EjecutarProcesoAsync(DateTime.Today, DateTime.Today);

                        Console.WriteLine($"[AUTO-PROC1] Ejecutado automáticamente a las {horaActual}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[AUTO-PROC1] ERROR: {ex.Message}");
                    }

                    // Evita doble ejecución dentro del mismo minuto
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }

                // Revisión cada 30 segundos
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}

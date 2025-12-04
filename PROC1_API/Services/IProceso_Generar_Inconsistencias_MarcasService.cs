using PROC1_API.Entities;

namespace PROC1_API.Services
{
    public interface IProceso_Generar_Inconsistencias_MarcasService
    {

        Task<BusinessLogicResponse> EjecutarProcesoAsync(DateTime fechaInicio, DateTime fechaFin, int? areaId = null, int? usuarioId = null);

    }
}

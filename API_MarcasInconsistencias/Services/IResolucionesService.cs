// Services/IResolucionesService.cs
using API_MarcasInconsistencias.Entities;

namespace API_MarcasInconsistencias.Services
{
    public interface IResolucionesService
    {
        Task<ResolucionesPaginadas> ObtenerResolucionesAsync(ResolucionesFiltro filtro);
        Task<IEnumerable<ResolucionSolicitud>> ExportarResolucionesAsync(ResolucionesFiltro filtro);
    }
}

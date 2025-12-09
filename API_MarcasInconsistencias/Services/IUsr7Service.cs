using API_MarcasInconsistencias.Entities;

namespace API_MarcasInconsistencias.Services
{
    public interface IUsr7Service
    {
        Task<bool> CrearVacacionAsync(VacacionNuevaDto dto);
        Task<VacacionesPaginadas> ObtenerMisSolicitudesAsync(VacacionFiltro filtro);
        Task<bool> ProcesarVacacionAsync(VacacionProcesarDto dto);
    }
}

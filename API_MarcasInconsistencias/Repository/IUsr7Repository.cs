using API_MarcasInconsistencias.Entities;

namespace API_MarcasInconsistencias.Repository
{
    public interface IUsr7Repository
    {
        Task<bool> CrearVacacionAsync(VacacionNuevaDto dto);
        Task<(IEnumerable<Vacacion>, int)> ObtenerMisSolicitudesAsync(VacacionFiltro filtro);
        Task<bool> ProcesarVacacionAsync(VacacionProcesarDto dto);
    }
}

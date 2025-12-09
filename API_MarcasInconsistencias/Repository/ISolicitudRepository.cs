using API_MarcasInconsistencias.Entities;

namespace API_MarcasInconsistencias.Repository
{
    public interface ISolicitudRepository
    {
        Task<(IEnumerable<Solicitud>, int)> ObtenerSolicitudesAsync(SolicitudFiltro filtro);
        Task<SolicitudDetalle?> ObtenerDetalleAsync(int id);
        Task<bool> ProcesarSolicitudAsync(SolicitudProcesarDto dto);
    }
}

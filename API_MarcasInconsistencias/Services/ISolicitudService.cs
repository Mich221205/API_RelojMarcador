using API_MarcasInconsistencias.Entities;

namespace API_MarcasInconsistencias.Services
{
    public interface ISolicitudService
    {
        Task<SolicitudesPaginadas> ObtenerSolicitudesAsync(SolicitudFiltro filtro);
        Task<SolicitudDetalle?> ObtenerDetalleAsync(int id);
        Task<bool> ProcesarSolicitudAsync(SolicitudProcesarDto dto);
    }
}

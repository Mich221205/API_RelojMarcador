using API_MarcasInconsistencias.Entities;
using API_MarcasInconsistencias.Repository;

namespace API_MarcasInconsistencias.Services
{
    public class SolicitudService : ISolicitudService
    {
        private readonly ISolicitudRepository _repo;

        public SolicitudService(ISolicitudRepository repo)
        {
            _repo = repo;
        }

        public async Task<SolicitudesPaginadas> ObtenerSolicitudesAsync(SolicitudFiltro filtro)
        {
            var (datos, total) = await _repo.ObtenerSolicitudesAsync(filtro);

            var totalPaginas =
                (int)Math.Ceiling((double)total / filtro.PageSize);

            return new SolicitudesPaginadas
            {
                Datos = datos,
                PaginaActual = filtro.Pagina,
                TotalPaginas = totalPaginas,
                TotalRegistros = total
            };
        }

        public Task<SolicitudDetalle?> ObtenerDetalleAsync(int id)
        {
            return _repo.ObtenerDetalleAsync(id);
        }

        public Task<bool> ProcesarSolicitudAsync(SolicitudProcesarDto dto)
        {
            if (dto.Accion != "Aprobado" && dto.Accion != "Rechazado")
                throw new Exception("Acción inválida.");

            return _repo.ProcesarSolicitudAsync(dto);
        }
    }
}

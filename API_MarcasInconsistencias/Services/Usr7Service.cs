using API_MarcasInconsistencias.Entities;
using API_MarcasInconsistencias.Repository;

namespace API_MarcasInconsistencias.Services
{
    public class Usr7Service : IUsr7Service
    {
        private readonly IUsr7Repository _repo;

        public Usr7Service(IUsr7Repository repo)
        {
            _repo = repo;
        }

        public Task<bool> CrearVacacionAsync(VacacionNuevaDto dto)
        {
            return _repo.CrearVacacionAsync(dto);
        }

        public async Task<VacacionesPaginadas> ObtenerMisSolicitudesAsync(VacacionFiltro filtro)
        {
            var (datos, total) = await _repo.ObtenerMisSolicitudesAsync(filtro);

            var totalPaginas =
                (int)Math.Ceiling((double)total / filtro.PageSize);

            return new VacacionesPaginadas
            {
                Datos = datos,
                PaginaActual = filtro.Pagina,
                TotalPaginas = totalPaginas,
                TotalRegistros = total
            };
        }

        public Task<bool> ProcesarVacacionAsync(VacacionProcesarDto dto)
        {
            if (dto.Accion != "Aprobado" && dto.Accion != "Rechazado")
                throw new Exception("Acción inválida.");

            return _repo.ProcesarVacacionAsync(dto);
        }
    }
}

// Services/ResolucionesService.cs
using API_MarcasInconsistencias.Entities;
using API_MarcasInconsistencias.Repository;

namespace API_MarcasInconsistencias.Services
{
    public class ResolucionesService : IResolucionesService
    {
        private readonly IResolucionesRepository _repository;

        public ResolucionesService(IResolucionesRepository repository)
        {
            _repository = repository;
        }

        public async Task<ResolucionesPaginadas> ObtenerResolucionesAsync(ResolucionesFiltro filtro)
        {
            var (datos, total) = await _repository.ObtenerResolucionesAsync(filtro);

            var totalPaginas = (int)Math.Ceiling((double)total / filtro.RegistrosPorPagina);

            return new ResolucionesPaginadas
            {
                Datos = datos,
                PaginaActual = filtro.Pagina,
                TotalPaginas = totalPaginas,
                TotalRegistros = total
            };
        }

        public Task<IEnumerable<ResolucionSolicitud>> ExportarResolucionesAsync(ResolucionesFiltro filtro)
        {
            return _repository.ExportarResolucionesAsync(filtro);
        }
    }
}

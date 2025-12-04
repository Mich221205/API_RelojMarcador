using ADM16.Entities;
using ADM16.Repository;

namespace ADM16.Services
{
    public class MarcaService : IMarcaService
    {


        private readonly IMarcaRepository _repo;

        public MarcaService(IMarcaRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<Marca>> Reporte_Marcas(
            int page,
            int pageSize,
            string? identificacion,
            DateTime? fecha)
            => _repo.Reporte_Marcas(page, pageSize, identificacion, fecha);

        public Task<int> Contar_Reporte_Marca(
            string? identificacion,
            DateTime? fecha)
            => _repo.Contar_Reporte_Marca(identificacion, fecha);
    }

}


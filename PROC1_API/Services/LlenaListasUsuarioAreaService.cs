using PROC1_API.Repository;

namespace PROC1_API.Services
{
    public class LlenaListasUsuarioAreaService : ILlenaListasUsuarioAreasService
    {
        private readonly LlenaListasUsuarioArea _repo;

        public LlenaListasUsuarioAreaService(LlenaListasUsuarioArea repo)
        {
            _repo = repo;
        }

        // LISTAR AREAS CON FILTRO OPCIONAL
        public async Task<IEnumerable<Entities.Area>> ListarAreasAsync(string? filtro)
        {
            return await _repo.ListarAsync(filtro);
        }

        // LISTAR FUNCIONARIOS
        public async Task<IEnumerable<(int Id, string Nombre)>> ListarFuncionariosAsync()
        {
            return await _repo.ListarFuncionariosAsync();
        }



    }

}

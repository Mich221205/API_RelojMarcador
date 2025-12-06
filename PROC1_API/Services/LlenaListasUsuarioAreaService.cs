using PROC1_API.Entities;
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

        // ============================================================
        // LISTAR ÁREAS
        // ============================================================
        public async Task<BusinessLogicResponse> ListarAreasAsync(string? filtro)
        {
            try
            {
                var resultado = await _repo.ListarAsync(filtro);

                return new BusinessLogicResponse
                {
                    StatusCode = 200,
                    Message = "Áreas obtenidas correctamente.",
                    ResponseObject = resultado
                };
            }
            catch (Exception ex)
            {
                return new BusinessLogicResponse
                {
                    StatusCode = 500,
                    Message = $"Error al obtener las áreas: {ex.Message} | STACK: {ex.StackTrace}"
                };
            }
        }

        // ============================================================
        // LISTAR FUNCIONARIOS
        // ============================================================
        public async Task<BusinessLogicResponse> ListarFuncionariosAsync()
        {
            try
            {
                var resultado = await _repo.ListarFuncionariosAsync();

                return new BusinessLogicResponse
                {
                    StatusCode = 200,
                    Message = "Funcionarios obtenidos correctamente.",
                    ResponseObject = resultado
                };
            }
            catch (Exception ex)
            {
                return new BusinessLogicResponse
                {
                    StatusCode = 500,
                    Message = $"Error al obtener los funcionarios: {ex.Message} | STACK: {ex.StackTrace}"
                };
            }
        }
    }
}

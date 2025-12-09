using PROC1_API.Entities;

namespace PROC1_API.Services
{
    public interface ILlenaListasUsuarioAreasService
    {

        Task<BusinessLogicResponse> ListarAreasAsync(string? filtro);
        Task<BusinessLogicResponse> ListarFuncionariosAsync();


    }
}

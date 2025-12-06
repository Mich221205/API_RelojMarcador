namespace PROC1_API.Services
{
    public interface ILlenaListasUsuarioAreasService
    {

        Task<IEnumerable<Entities.Area>> ListarAreasAsync(string? filtro);
        Task<IEnumerable<(int Id, string Nombre)>> ListarFuncionariosAsync();


    }
}

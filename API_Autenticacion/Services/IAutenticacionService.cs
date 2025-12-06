namespace API_Autenticacion.Services
{
    public interface IAutenticacionService
    {
        Task<LoginResultado> LoginAsync(string usuario, string contrasenna);
    }
}

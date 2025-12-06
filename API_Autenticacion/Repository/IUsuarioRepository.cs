using API_Autenticacion.Entities;

namespace API_Autenticacion.Repository
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObtenerPorIdentificacionAsync(string identificacion);
        Task BloquearUsuarioAsync(int idUsuario);
    }
}

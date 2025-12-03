using MarcasInconsistencias.Entities;

namespace MarcasInconsistencias.Services
{
    public interface IMarcaService
    {
        Task<IEnumerable<Marca>> GetByUsuarioAsync(int idUsuario, int idUsuarioAccion);
    }
}

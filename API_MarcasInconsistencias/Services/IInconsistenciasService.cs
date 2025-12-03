using MarcasInconsistencias.Entities;

namespace MarcasInconsistencias.Services
{
    public interface IInconsistenciasService
    {
        Task<IEnumerable<InconsistenciaReporte>> GetAllAsync(int idUsuarioAccion);
    }
}

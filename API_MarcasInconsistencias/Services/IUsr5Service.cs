using API_MarcasInconsistencias.Entities;

namespace API_MarcasInconsistencias.Services
{
    public interface IUsr5Service
    {
        Task<IEnumerable<InconsistenciaPendiente>> ObtenerInconsistenciasAsync(string identificacion);
        Task<IEnumerable<MotivoAusencia>> ObtenerMotivosAsync();

        Task<(bool ok, List<string> errores)> CrearJustificacionAsync(
            JustificacionCrearDto dto,
            string? adjuntoUrl
        );
    }
}

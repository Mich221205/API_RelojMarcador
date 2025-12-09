using API_MarcasInconsistencias.Entities;

namespace API_MarcasInconsistencias.Repository
{
    public interface IUsr5Repository
    {
        Task<IEnumerable<InconsistenciaPendiente>> ObtenerInconsistenciasUsuarioAsync(string identificacion);
        Task<IEnumerable<MotivoAusencia>> ObtenerMotivosAsync();

        Task<bool> InconsistenciaPerteneceAlUsuarioAsync(int idInconsistencia, string identificacion);
        Task<bool> ExisteJustificacionAsync(int idInconsistencia);

        Task CrearJustificacionAsync(JustificacionCrearDto dto, string? adjuntoUrl, string descripcionBitacora);
    }
}

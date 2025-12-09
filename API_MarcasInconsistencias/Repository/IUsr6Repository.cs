using API_MarcasInconsistencias.Entities;

namespace API_MarcasInconsistencias.Repository
{
    public interface IUsr6Repository
    {
        Task<IEnumerable<MotivoAusencia>> ObtenerMotivosAsync();
        Task<bool> TieneVacacionesSuperpuestasAsync(int idUsuario, DateTime fechaInicio, DateTime fechaFin);
        Task CrearPermisoAsync(PermisoCrearDto dto, string? adjuntoUrl, string descripcionBitacoraJson);
        Task<IEnumerable<PermisoListDto>> ObtenerPermisosAsync(PermisoFiltroDto filtro);
    }
}

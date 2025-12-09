using API_MarcasInconsistencias.Entities;

namespace API_MarcasInconsistencias.Services
{
    public interface IUsr6Service
    {
        Task<IEnumerable<MotivoAusencia>> ObtenerMotivosAsync();
        Task<(bool ok, List<string> errores)> CrearPermisoAsync(PermisoCrearDto dto, string? adjuntoUrl);
        Task<IEnumerable<PermisoListDto>> ObtenerPermisosAsync(PermisoFiltroDto filtro);
    }
}

using ADM16.Entities;

namespace ADM16.Services
{
    public interface IMarcaService
    {
        Task<IEnumerable<Marca>> Reporte_Marcas(
            int page,
            int pageSize,
            string? identificacion,
            DateTime? fecha);

        Task<int> Contar_Reporte_Marca(
            string? identificacion,
            DateTime? fecha);
    }
}

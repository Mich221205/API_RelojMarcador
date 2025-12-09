namespace API_MarcasInconsistencias.Entities
{
    public class VacacionesPaginadas
    {
        public IEnumerable<Vacacion> Datos { get; set; } = new List<Vacacion>();
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }
    }
}

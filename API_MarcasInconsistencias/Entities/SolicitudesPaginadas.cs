namespace API_MarcasInconsistencias.Entities
{
    public class SolicitudesPaginadas
    {
        public IEnumerable<Solicitud> Datos { get; set; } = new List<Solicitud>();
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }
    }
}

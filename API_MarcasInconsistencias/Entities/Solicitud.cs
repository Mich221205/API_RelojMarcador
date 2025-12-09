namespace API_MarcasInconsistencias.Entities
{
    public class Solicitud
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = "";
        public string Solicitante { get; set; } = "";
        public string Identificacion { get; set; } = "";
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string Decision { get; set; } = "";
        public string? Observacion { get; set; }
        public DateTime? FechaResolucion { get; set; }
    }
}

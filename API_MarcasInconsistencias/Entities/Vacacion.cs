namespace API_MarcasInconsistencias.Entities
{
    public class Vacacion
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = "Vacaciones";
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Estado { get; set; } = "";
        public string? Observacion { get; set; }
        public DateTime? FechaResolucion { get; set; }
    }
}

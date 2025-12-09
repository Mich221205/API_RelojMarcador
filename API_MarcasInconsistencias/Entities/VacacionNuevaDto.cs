namespace API_MarcasInconsistencias.Entities
{
    public class VacacionNuevaDto
    {
        public int IdSolicitante { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string? Motivo { get; set; }
        public string? Adjuntos { get; set; }
    }
}

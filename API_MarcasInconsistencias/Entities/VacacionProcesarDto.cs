namespace API_MarcasInconsistencias.Entities
{
    public class VacacionProcesarDto
    {
        public int IdSolicitud { get; set; }
        public string Accion { get; set; } = ""; // Aprobado / Rechazado
        public string? Observacion { get; set; }
    }
}

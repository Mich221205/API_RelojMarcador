namespace API_MarcasInconsistencias.Entities
{
    public class PermisoCrearDto
    {
        public int IdUsuario { get; set; }
        public string Identificacion { get; set; } = string.Empty; // por si luego lo necesitas
        public DateTime FechaHoraInicio { get; set; }
        public DateTime FechaHoraFin { get; set; }
        public int IdMotivo { get; set; }
        public string? Observaciones { get; set; }
    }
}

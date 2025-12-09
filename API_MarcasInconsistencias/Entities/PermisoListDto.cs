namespace API_MarcasInconsistencias.Entities
{
    public class PermisoListDto
    {
        public int IdPermiso { get; set; }
        public DateTime FechaInicio { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public TimeSpan HoraFin { get; set; }

        public int IdMotivo { get; set; }
        public string NombreMotivo { get; set; } = string.Empty;

        public string? Observaciones { get; set; }
        public string? AdjuntoUrl { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaSolicitud { get; set; }
    }
}

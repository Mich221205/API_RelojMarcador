namespace API_MarcasInconsistencias.Entities
{
    public class InconsistenciaPendiente
    {
        public int IdInconsistenciaUsuario { get; set; }
        public DateTime FechaInconsistencia { get; set; }
        public string NombreInconsistencia { get; set; } = string.Empty;
    }
}

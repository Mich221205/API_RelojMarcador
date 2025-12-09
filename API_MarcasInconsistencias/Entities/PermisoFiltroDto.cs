namespace API_MarcasInconsistencias.Entities
{
    public class PermisoFiltroDto
    {
        public int IdUsuario { get; set; }
        public string Estado { get; set; } = "Todos"; // Pendiente/Aprobado/Rechazado/Todos
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
    }
}

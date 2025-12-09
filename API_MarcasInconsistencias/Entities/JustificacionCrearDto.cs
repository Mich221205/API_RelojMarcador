namespace API_MarcasInconsistencias.Entities
{
    public class JustificacionCrearDto
    {
        public int IdInconsistenciaUsuario { get; set; }
        public int IdMotivo { get; set; }
        public string Descripcion { get; set; } = string.Empty;

        // Usuario que justifica
        public int IdUsuario { get; set; }
        public string Identificacion { get; set; } = string.Empty;
    }
}

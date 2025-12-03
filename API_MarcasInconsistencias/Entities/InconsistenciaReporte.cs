namespace MarcasInconsistencias.Entities
{
    public class InconsistenciaReporte
    {
        public int ID_Inconsistencia { get; set; }
        public int ID_Usuario { get; set; }
        public string Nombre_Inconsistencia { get; set; } = null!;
        public DateTime Fecha_Inconsistencia { get; set; }
        public string Estado { get; set; } = null!;
        public string? Detalle { get; set; }
        public string? Referencia { get; set; }
    }
}

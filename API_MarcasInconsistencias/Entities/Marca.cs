namespace MarcasInconsistencias.Entities
{
    public class Marca
    {
        public int ID_Marca { get; set; }
        public int ID_Usuario { get; set; }
        public int ID_Area { get; set; }
        public string? Detalle { get; set; }
        public string? Tipo_Marca { get; set; }
        public DateTime Fecha_Hora { get; set; }
        public string IP_Usuario { get; set; } = null!;
        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }
        public string Ciudad { get; set; } = null!;
        public string Direccion { get; set; } = null!;
        public string Pais { get; set; } = null!;
    }
}

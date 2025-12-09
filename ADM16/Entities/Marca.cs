namespace ADM16.Entities
{
    public class Marca : Usuario
    {
        public int ID_Marca { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Detalle { get; set; } = string.Empty;
        public string Tipo_Marca { get; set; } = string.Empty;
        public DateTime Fecha_Hora { get; set; }
        public string IP_Usuario { get; set; } = string.Empty;
        public string Latitud { get; set; } = string.Empty;
        public string Longitud { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Pais { get; set; } = string.Empty;
    }
    public class ReporteMarcasResponse
    {
        public int TotalRegistros { get; set; }
        public int Pagina { get; set; }
        public int PorPagina { get; set; }
        public IEnumerable<Marca> Data { get; set; } = new List<Marca>();
    }


}

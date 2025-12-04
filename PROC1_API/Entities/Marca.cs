namespace PROC1_API.Entities
{
    public class Marca : Usuario
    {
        public int ID_Marca { get; set; }

        public string Usuario { get; set; }

        public string Area { get; set; }

        public string Detalle { get; set; }

        public string Tipo_Marca { get; set; }

        public DateTime Fecha_Hora { get; set; }

        public string IP_Usuario { get; set; }

        public string Latitud { get; set; }

        public string Longitud { get; set; }

        public string Ciudad { get; set; }

        public string Direccion { get; set; }
        public string Pais { get; set; }

    }
}

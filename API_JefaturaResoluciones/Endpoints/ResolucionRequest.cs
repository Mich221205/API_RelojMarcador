namespace API_JefaturaResoluciones.Entities
{

    public class ResolucionRequest
    {

        public int IdRegistro { get; set; }

        public int IdJefatura { get; set; }

        public string Decision { get; set; } = string.Empty;

        public string Observacion { get; set; } = string.Empty;
    }
}

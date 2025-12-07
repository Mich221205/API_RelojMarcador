namespace API_JefaturaResoluciones.Entities;

public class ResolucionRow
{
    public string Tipo { get; set; } = "";       
    public int IdRegistro { get; set; }         
    public string Funcionario { get; set; } = "";
    public DateTime FechaDesde { get; set; }
    public DateTime FechaHasta { get; set; }
    public string Decision { get; set; } = "";      
    public DateTime FechaResolucion { get; set; }
    public string Observacion { get; set; } = "";
}

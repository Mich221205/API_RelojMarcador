namespace API_JefaturaResoluciones.Entities;

public class PermisoRow
{
    public int ID_Permiso { get; set; }
    public int ID_Usuario { get; set; }
    public DateTime Fecha_Inicio { get; set; }
    public TimeSpan Hora_Inicio { get; set; }
    public DateTime Fecha_Fin { get; set; }
    public TimeSpan Hora_Fin { get; set; }
    public int ID_Motivo { get; set; }
    public string? Observaciones { get; set; }
    public string? Adjunto_URL { get; set; }
    public string Estado { get; set; } = "";
    public DateTime Fecha_Solicitud { get; set; }

    public string Funcionario { get; set; } = "";
    public string Tipo { get; set; } = "Permiso"; 
}

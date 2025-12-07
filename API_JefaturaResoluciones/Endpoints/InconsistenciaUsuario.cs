namespace API_JefaturaResoluciones.Entities;

public class InconsistenciaUsuarioRow
{
    public int ID_Inconsistencia_Usuario { get; set; }
    public string Identificacion { get; set; } = "";
    public int ID_Inconsistencia { get; set; }
    public DateTime Fecha_Inconsistencia { get; set; }
    public string Estado { get; set; } = "";
    public string? Detalle { get; set; }
    public string? Referencia { get; set; }

    public string Funcionario { get; set; } = "";
    public string Tipo_Inconsistencia { get; set; } = "";
}

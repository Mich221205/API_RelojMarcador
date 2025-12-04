namespace PROC1_API.Entities
{
    public class Inconsistencias
    {
        public int ID_Inconsistencia { get; set; }
        public string Nombre_Inconsistencia { get; set; }

    }

    public class ParametrosInconsistencia
    {
        public int Tolerancia_Atraso { get; set; }
        public int Tolerancia_Salida_Temprana { get; set; }
    }

    public class Exclusion_Inconsistencia
    {
        public bool Exclusion { get; set; }
        public string Detalle { get; set; }
    }

    public class InconsistenciaDetectada
    {
        public string Tipo { get; set; }
        public DateTime Fecha { get; set; }
        public int ID_Usuario { get; set; }
        public string? Detalle { get; set; }
        public string? Referencia { get; set; }

        public InconsistenciaDetectada(string tipo, DateTime fecha, int idUsuario, string? detalle = null, string? referencia = null)
        {
            Tipo = tipo;
            Fecha = fecha;
            ID_Usuario = idUsuario;
            Detalle = detalle;
            Referencia = referencia;
        }
    
    }


}

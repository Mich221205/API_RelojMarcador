// Entities/ResolucionSolicitud.cs
namespace API_MarcasInconsistencias.Entities
{
    public class ResolucionSolicitud
    {
        public int Id { get; set; }                 // s.ID
        public string Tipo { get; set; } = "";      // s.Tipo
        public string Solicitante { get; set; } = "";// CONCAT(nombre + apellidos)
        public string Identificacion { get; set; } = ""; // u.Identificacion
        public DateTime? FechaInicio { get; set; }  // s.Fecha_Inicio
        public DateTime? FechaFin { get; set; }     // s.Fecha_Fin
        public string Decision { get; set; } = "";  // s.Decision
        public DateTime? FechaResolucion { get; set; } // s.Fecha_Resolucion
        public string? Observacion { get; set; }    // s.Observacion
    }
}

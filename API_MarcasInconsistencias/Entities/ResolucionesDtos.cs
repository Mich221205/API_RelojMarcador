// Entities/ResolucionesDtos.cs
namespace API_MarcasInconsistencias.Entities
{
    // Filtros equivalentes a $_GET del PHP
    public class ResolucionesFiltro
    {
        public string? Tipo { get; set; }           // "Todas", "Permiso", etc.
        public string? Estado { get; set; }         // "Aprobado" / "Rechazado" / null
        public string? Funcionario { get; set; }    // nombre/apellido/ID
        public DateTime? FechaInicio { get; set; }  // fecha_inicio
        public DateTime? FechaFin { get; set; }     // fecha_fin
        public int Pagina { get; set; } = 1;        // página actual
        public int RegistrosPorPagina { get; set; } = 10;
    }

    public class ResolucionesPaginadas
    {
        public IEnumerable<ResolucionSolicitud> Datos { get; set; }
            = new List<ResolucionSolicitud>();
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }
    }
}

namespace API_MarcasInconsistencias.Entities
{
    public class SolicitudFiltro
    {
        public string? Tipo { get; set; }
        public string? Estado { get; set; }
        public string? Funcionario { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        public int Pagina { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

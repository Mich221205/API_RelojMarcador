namespace API_MarcasInconsistencias.Entities
{
    public class VacacionFiltro
    {
        public int IdUsuario { get; set; }
        public int Pagina { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

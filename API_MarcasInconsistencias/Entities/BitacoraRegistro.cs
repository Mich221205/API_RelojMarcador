namespace MarcasInconsistencias.Entities
{
    public class BitacoraRegistro
    {
        public int ID_Registro { get; set; }
        public DateTime Fecha_Registro { get; set; }
        public int ID_Usuario { get; set; }
        public int ID_Accion { get; set; }
        public string Descripcion_Accion { get; set; } = null!;
    }
}

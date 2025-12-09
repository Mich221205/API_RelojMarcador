namespace ADM16.Entities
{
    public class Usuario
    {
        public int ID_Usuario { get; set; }
        public string Identificacion { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido_1 { get; set; } = string.Empty;
        public string Apellido_2 { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public int Id_Rol_Usuario { get; set; }
        public string Contrasena { get; set; } = string.Empty;
        public bool Estado { get; set; }
    }
}

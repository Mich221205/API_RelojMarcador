namespace API_Autenticacion.Entities
{
    public class Usuario
    {
        public int ID_Usuario { get; set; }
        public int ID_Tipo_Identificacion { get; set; }
        public string Identificacion { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido_1 { get; set; } = string.Empty;
        public string Apellido_2 { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public int ID_Rol_Usuario { get; set; }
        public string Contrasena { get; set; } = string.Empty;
        public DateTime Fecha_Creacion { get; set; }
        public bool Estado { get; set; }          // 1 = activo, 0 = bloqueado
    }
}

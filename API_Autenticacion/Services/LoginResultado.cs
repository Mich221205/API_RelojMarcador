using API_Autenticacion.Entities;

namespace API_Autenticacion.Services
{
    public enum LoginEstado
    {
        Exitoso,
        CredencialesInvalidas,
        UsuarioBloqueado,
        Error
    }

    public class LoginResultado
    {
        public LoginEstado Estado { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public Usuario? Usuario { get; set; }
    }
}

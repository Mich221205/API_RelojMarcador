using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using API_Autenticacion.Entities;
using API_Autenticacion.Repository;

namespace API_Autenticacion.Services
{
    public class AutenticacionService : IAutenticacionService
    {
        // misma clave/IV que en PHP
        private const string AES_KEY = "12345678901234567890123456789012";
        private const string AES_IV = "1234567890123456";

        private readonly IUsuarioRepository _usuarioRepository;
        private static readonly ConcurrentDictionary<string, int> _intentos =
            new ConcurrentDictionary<string, int>();

        public AutenticacionService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<LoginResultado> LoginAsync(string usuario, string contrasenna)
        {
            var resultado = new LoginResultado();

            try
            {
                var u = await _usuarioRepository.ObtenerPorIdentificacionAsync(usuario);

                if (u == null)
                {
                    resultado.Estado = LoginEstado.CredencialesInvalidas;
                    resultado.Mensaje = "Usuario y/o contraseña incorrectos.";
                    return resultado;
                }

                if (!u.Estado)
                {
                    resultado.Estado = LoginEstado.UsuarioBloqueado;
                    resultado.Mensaje = "Usuario bloqueado.";
                    return resultado;
                }

                var contrasennaDescifrada = DesencriptarAes(u.Contrasena);

                if (contrasennaDescifrada != contrasenna)
                {
                    var intentos = _intentos.AddOrUpdate(
                        u.Identificacion,
                        1,
                        (_, actual) => actual + 1
                    );

                    if (intentos >= 3)
                    {
                        await _usuarioRepository.BloquearUsuarioAsync(u.ID_Usuario);
                        _intentos.TryRemove(u.Identificacion, out _);

                        resultado.Estado = LoginEstado.UsuarioBloqueado;
                        resultado.Mensaje = "Usuario bloqueado por múltiples intentos fallidos.";
                        return resultado;
                    }

                    resultado.Estado = LoginEstado.CredencialesInvalidas;
                    resultado.Mensaje = "Usuario y/o contraseña incorrectos.";
                    return resultado;
                }

                
                _intentos.TryRemove(u.Identificacion, out _);

                resultado.Estado = LoginEstado.Exitoso;
                resultado.Usuario = u;
                resultado.Mensaje = "Ok";
                return resultado;
            }
            catch
            {
                resultado.Estado = LoginEstado.Error;
                resultado.Mensaje = "Error interno al procesar el inicio de sesión.";
                return resultado;
            }
        }

        private static string DesencriptarAes(string base64CipherText)
        {
            var cipherBytes = Convert.FromBase64String(base64CipherText);

            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(AES_KEY);
            aes.IV = Encoding.UTF8.GetBytes(AES_IV);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(cipherBytes);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs, Encoding.UTF8);
            return sr.ReadToEnd();
        }
    }
}

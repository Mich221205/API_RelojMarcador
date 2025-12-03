using Dapper;
using System.Data;
using System.Text.Json;

namespace MarcasInconsistencias.Repository
{
    public class BitacoraRepository
    {
        private readonly IDbConnectionFactory _db;

        public BitacoraRepository(IDbConnectionFactory db)
        {
            _db = db;
        }

        public async Task RegistrarConsultaAsync(int idUsuario, string descripcion)
        {
            using var conn = _db.CreateConnection();

            try
            {
                var descripcionJson = JsonSerializer.Serialize(new
                {
                    Accion = "CONSULTA",
                    Detalle = descripcion
                });

                Console.WriteLine("📝 Registrando bitácora...");
                Console.WriteLine($"Usuario: {idUsuario}");
                Console.WriteLine($"JSON: {descripcionJson}");

                const string sql = @"
                    INSERT INTO bitacora
                        (Fecha_Registro, ID_Usuario, ID_Accion, Descripcion_Accion)
                    VALUES
                        (CURDATE(), @idUsuario, 4, @descripcionJson);";

                await conn.ExecuteAsync(sql, new { idUsuario, descripcionJson });
            }
            catch (Exception ex)
            {
                Console.WriteLine("🔥 ERROR EN BITÁCORA:");
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

    }
}

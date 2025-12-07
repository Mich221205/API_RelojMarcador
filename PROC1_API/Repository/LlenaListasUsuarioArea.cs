using Dapper;
using PROC1_API.Entities;

namespace PROC1_API.Repository
{
    public class LlenaListasUsuarioArea
    {

        private readonly IDbConnectionFactory _dbConnectionFactory;

        public LlenaListasUsuarioArea(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }


        // LISTAR AREAS CON FILTRO OPCIONAL
        public async Task<IEnumerable<Area>> ListarAsync(string? filtro)
        {
            using var con = _dbConnectionFactory.CreateConnection();

            // Filtro opcional por nombre área, código, identificación y nombre del jefe
            var where = @"
            (@f IS NULL
             OR a.Nombre_Area LIKE CONCAT('%',@f,'%')
             OR a.Codigo_Area LIKE CONCAT('%',@f,'%')
             OR u.Identificacion LIKE CONCAT('%',@f,'%')
             OR CONCAT(u.Nombre,' ',u.Apellido_1,' ',u.Apellido_2) LIKE CONCAT('%',@f,'%')
            )";

                        var sql = $@"
            SELECT 
                a.ID_Area,
                a.Nombre_Area,
                a.Jefe_Area,
                a.Codigo_Area,
                CONCAT(u.Nombre,' ',u.Apellido_1,' ',u.Apellido_2) AS Jefe_Nombre
            FROM areas a
            LEFT JOIN usuario u ON u.ID_Usuario = a.Jefe_Area
            WHERE {where}
            ORDER BY a.Nombre_Area;";

                        return await con.QueryAsync<Area>(sql, new { f = string.IsNullOrWhiteSpace(filtro) ? null : filtro!.Trim() });
        }


        // LISTAR FUNCIONARIOS

        public async Task<IEnumerable<FuncionarioListItem>> ListarFuncionariosAsync()
        {
            using var con = _dbConnectionFactory.CreateConnection();

            const string sql = @"
                                SELECT 
                                    u.ID_Usuario      AS ID_Usuario,
                                    CONCAT(u.Nombre,' ',u.Apellido_1,' ',u.Apellido_2,' (',u.Identificacion,')') AS Nombre
                                FROM usuario u
                                WHERE u.Estado = 1
                                ORDER BY u.Nombre, u.Apellido_1, u.Apellido_2;";

            return await con.QueryAsync<FuncionarioListItem>(sql);
        }

    }


}


using System.Data;
using Dapper;
using MySql.Data.MySqlClient;
using API_JefaturaResoluciones.Entities;

namespace API_JefaturaResoluciones.Repository;

public class JefaturaRepository
{
    private readonly string _connectionString;

    public JefaturaRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("RelojMarcadorDb")
            ?? throw new InvalidOperationException("Falta ConnectionStrings:RelojMarcadorDb");
    }

    private IDbConnection CreateConnection()
        => new MySqlConnection(_connectionString);


    public async Task<IEnumerable<PermisoRow>> GetPermisosPendientesAsync(
        int? idUsuario, DateTime? desde, DateTime? hasta, string? estado, bool soloVacaciones)
    {
        using var conn = CreateConnection();

        var sql = @"
            SELECT p.*, 
                   CONCAT(u.Nombre, ' ', u.Apellido_1, ' ', u.Apellido_2) AS Funcionario,
                   CASE WHEN p.ID_Motivo = 2 THEN 'Vacación' ELSE 'Permiso' END AS Tipo
            FROM permisos p
            JOIN usuario u ON u.ID_Usuario = p.ID_Usuario
            WHERE (@estado IS NULL OR p.Estado = @estado)
              AND (@idUsuario IS NULL OR p.ID_Usuario = @idUsuario)
              AND (@desde IS NULL OR p.Fecha_Inicio >= @desde)
              AND (@hasta IS NULL OR p.Fecha_Fin   <= @hasta)
              AND (@soloVac = 0 OR p.ID_Motivo = 2)
              AND (@soloPerm = 0 OR p.ID_Motivo <> 2)
            ORDER BY p.Fecha_Solicitud DESC;
        ";

        var result = await conn.QueryAsync<PermisoRow>(sql, new
        {
            idUsuario,
            desde,
            hasta,
            estado,
            soloVac = soloVacaciones ? 1 : 0,
            soloPerm = soloVacaciones ? 0 : 1
        });

        return result;
    }

    public async Task<IEnumerable<InconsistenciaUsuarioRow>> GetJustificacionesPendientesAsync(
        int? idUsuario, DateTime? desde, DateTime? hasta, string? estado)
    {
        using var conn = CreateConnection();

        var sql = @"
            SELECT iu.*,
                   CONCAT(u.Nombre, ' ', u.Apellido_1, ' ', u.Apellido_2) AS Funcionario,
                   i.Nombre_Inconsistencia AS Tipo_Inconsistencia
            FROM inconsistencias_usuario iu
            JOIN usuario u ON u.Identificacion = iu.Identificacion
            JOIN inconsistencia i ON i.ID_Inconsistencia = iu.ID_Inconsistencia
            WHERE (@estado IS NULL OR iu.Estado = @estado)
              AND (@idUsuario IS NULL OR u.ID_Usuario = @idUsuario)
              AND (@desde IS NULL OR iu.Fecha_Inconsistencia >= @desde)
              AND (@hasta IS NULL OR iu.Fecha_Inconsistencia <= @hasta)
            ORDER BY iu.Fecha_Inconsistencia DESC;
        ";

        return await conn.QueryAsync<InconsistenciaUsuarioRow>(sql, new
        {
            idUsuario,
            desde,
            hasta,
            estado
        });
    }

    public async Task ResolverPermisoAsync(int idPermiso, string decision, string observacion, int idJefatura)
    {
        using var conn = CreateConnection();
        conn.Open();
        using var trx = conn.BeginTransaction();

        var permiso = await conn.QuerySingleAsync<PermisoRow>(
            "SELECT * FROM permisos WHERE ID_Permiso = @id",
            new { id = idPermiso }, trx);

        await conn.ExecuteAsync(@"
        UPDATE permisos
           SET Estado = @decision,
               Observaciones = CONCAT(COALESCE(Observaciones, ''), '\n[Jefatura] ', @obs)
         WHERE ID_Permiso = @idPermiso;
    ", new { decision, obs = observacion, idPermiso }, trx);

        if (decision == "Aprobado")
        {
            try
            {
                await conn.ExecuteAsync(
                    "CALL sp_marcar_dias_no_computables(@idUsuario, @fechaIni, @fechaFin);",
                    new
                    {
                        idUsuario = permiso.ID_Usuario,
                        fechaIni = permiso.Fecha_Inicio,
                        fechaFin = permiso.Fecha_Fin
                    },
                    trx
                );
            }
            catch (Exception ex)
            {
                // Log si quieres, pero NO rompas la transacción
                Console.Error.WriteLine($"Error en sp_marcar_dias_no_computables: {ex.Message}");
            }
        }

        var payload = new
        {
            Modulo = "JEF1",
            Tipo = "Permiso",
            ID_Permiso = idPermiso,
            Decision = decision,
            Observacion = observacion,
            Antes = permiso,
            Despues = new { permiso.ID_Permiso, Estado = decision }
        };
        var json = System.Text.Json.JsonSerializer.Serialize(payload);

        await conn.ExecuteAsync(@"
        INSERT INTO bitacora (Fecha_Registro, ID_Usuario, ID_Accion, Descripcion_Accion)
        VALUES (NOW(), @idJefatura, 5, @json);
    ", new { idJefatura, json }, trx);

        trx.Commit();
    }


    public async Task ResolverJustificacionAsync(int idIncUser, string decision, string observacion, int idJefatura)
    {
        using var conn = CreateConnection();
        conn.Open();
        using var trx = conn.BeginTransaction();

        var inc = await conn.QuerySingleAsync<InconsistenciaUsuarioRow>(
            "SELECT * FROM inconsistencias_usuario WHERE ID_Inconsistencia_Usuario = @id",
            new { id = idIncUser }, trx);

        await conn.ExecuteAsync(@"
            UPDATE inconsistencias_usuario
               SET Estado = @decision,
                   Detalle = CONCAT(COALESCE(Detalle, ''), '\n[Jefatura] ', @obs)
             WHERE ID_Inconsistencia_Usuario = @id;
        ", new { decision, obs = observacion, id = idIncUser }, trx);

        var payload = new
        {
            Modulo = "JEF1",
            Tipo = "Justificacion",
            ID_Inconsistencia_Usuario = idIncUser,
            Decision = decision,
            Observacion = observacion,
            Antes = inc,
            Despues = new { inc.ID_Inconsistencia_Usuario, Estado = decision }
        };
        var json = System.Text.Json.JsonSerializer.Serialize(payload);

        await conn.ExecuteAsync(@"
            INSERT INTO bitacora (Fecha_Registro, ID_Usuario, ID_Accion, Descripcion_Accion)
            VALUES (NOW(), @idJefatura, 5, @json);
        ", new { idJefatura, json }, trx);

        trx.Commit();
    }


    public async Task<(IEnumerable<ResolucionRow> Items, int Total)> GetResolucionesAsync(
        string? tipo, string? estado, int? idUsuario, DateTime? desde, DateTime? hasta,
        int page, int pageSize)
    {
        using var conn = CreateConnection();

        // Subquery base para NO duplicar código entre sql y countSql
        var baseSubquery = @"
        SELECT 'Permiso' AS Tipo,
               p.ID_Permiso AS IdRegistro,
               CONCAT(u.Nombre, ' ', u.Apellido_1, ' ', u.Apellido_2) AS Funcionario,
               p.Fecha_Inicio AS FechaDesde,
               p.Fecha_Fin AS FechaHasta,
               p.Estado AS Decision,
               p.Fecha_Solicitud AS FechaResolucion,
               p.Observaciones AS Observacion
        FROM permisos p
        JOIN usuario u ON u.ID_Usuario = p.ID_Usuario
        WHERE p.Estado IN ('Aprobado','Rechazado')

        UNION ALL

        SELECT 'Justificación' AS Tipo,
               iu.ID_Inconsistencia_Usuario AS IdRegistro,
               CONCAT(u.Nombre, ' ', u.Apellido_1, ' ', u.Apellido_2) AS Funcionario,
               iu.Fecha_Inconsistencia AS FechaDesde,
               iu.Fecha_Inconsistencia AS FechaHasta,
               iu.Estado AS Decision,
               iu.Fecha_Inconsistencia AS FechaResolucion,
               iu.Detalle AS Observacion
        FROM inconsistencias_usuario iu
        JOIN usuario u ON u.Identificacion = iu.Identificacion
        WHERE iu.Estado IN ('Aprobada','Rechazada')
    ";

        var sql = $@"
        SELECT * FROM (
            {baseSubquery}
        ) t
        WHERE (@tipo IS NULL OR t.Tipo = @tipo)
          AND (@estado IS NULL OR t.Decision = @estado)
          AND (@idUsuario IS NULL OR t.Funcionario LIKE CONCAT('%', @idUsuario, '%'))
          AND (@desde IS NULL OR t.FechaDesde >= @desde)
          AND (@hasta IS NULL OR t.FechaHasta <= @hasta)
        ORDER BY t.FechaResolucion DESC
        LIMIT @skip, @take;
    ";

        var countSql = $@"
        SELECT COUNT(*) FROM (
            {baseSubquery}
        ) x
        WHERE (@tipo IS NULL OR x.Tipo = @tipo)
          AND (@estado IS NULL OR x.Decision = @estado)
          AND (@idUsuario IS NULL OR x.Funcionario LIKE CONCAT('%', @idUsuario, '%'))
          AND (@desde IS NULL OR x.FechaDesde >= @desde)
          AND (@hasta IS NULL OR x.FechaHasta <= @hasta);
    ";

        int skip = (page - 1) * pageSize;

        var items = await conn.QueryAsync<ResolucionRow>(sql, new
        {
            tipo,
            estado,
            idUsuario,
            desde,
            hasta,
            skip,
            take = pageSize
        });

        var total = await conn.ExecuteScalarAsync<int>(countSql, new
        {
            tipo,
            estado,
            idUsuario,
            desde,
            hasta
        });

        return (items, total);
    }
}


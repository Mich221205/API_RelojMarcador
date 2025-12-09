using System.Text.Json;
using API_MarcasInconsistencias.Entities;
using API_MarcasInconsistencias.Repository;

namespace API_MarcasInconsistencias.Services
{
    public class Usr5Service : IUsr5Service
    {
        private readonly IUsr5Repository _repo;

        public Usr5Service(IUsr5Repository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<InconsistenciaPendiente>> ObtenerInconsistenciasAsync(string identificacion)
            => _repo.ObtenerInconsistenciasUsuarioAsync(identificacion);

        public Task<IEnumerable<MotivoAusencia>> ObtenerMotivosAsync()
            => _repo.ObtenerMotivosAsync();

        public async Task<(bool ok, List<string> errores)> CrearJustificacionAsync(
            JustificacionCrearDto dto,
            string? adjuntoUrl
        )
        {
            var errores = new List<string>();

            if (dto.IdInconsistenciaUsuario <= 0)
                errores.Add("Seleccione la inconsistencia.");

            if (dto.IdMotivo <= 0)
                errores.Add("Seleccione el motivo.");

            if (string.IsNullOrWhiteSpace(dto.Descripcion))
                errores.Add("La descripción es obligatoria.");

            if (dto.Descripcion?.Length > 300)
                errores.Add("La descripción no puede exceder 300 caracteres.");

            if (string.IsNullOrWhiteSpace(dto.Identificacion))
                errores.Add("Identificación de usuario requerida.");

            if (dto.IdUsuario <= 0)
                errores.Add("ID de usuario inválido.");

            if (errores.Count > 0)
                return (false, errores);

            // Validación en BD: inconsistencia pertenece y está en estado válido
            var pertenece = await _repo.InconsistenciaPerteneceAlUsuarioAsync(
                dto.IdInconsistenciaUsuario,
                dto.Identificacion
            );

            if (!pertenece)
            {
                errores.Add("La inconsistencia no es válida o no pertenece a su usuario.");
                return (false, errores);
            }

            // Validación en BD: no exista justificación previa
            var yaTieneJust = await _repo.ExisteJustificacionAsync(dto.IdInconsistenciaUsuario);
            if (yaTieneJust)
            {
                errores.Add("Ya existe una justificación para esta inconsistencia.");
                return (false, errores);
            }

            // Construir JSON para bitácora
            var payload = new
            {
                Modulo = "USR5",
                Accion = "CREAR_JUSTIFICACION",
                Inc = dto.IdInconsistenciaUsuario,
                Motivo = dto.IdMotivo,
                Adjunto = !string.IsNullOrEmpty(adjuntoUrl)
            };

            var descBitacora = JsonSerializer.Serialize(
                payload,
                new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping }
            );

            // Registrar justificación + actualizar inconsistencia + bitácora
            await _repo.CrearJustificacionAsync(dto, adjuntoUrl, descBitacora);

            return (true, errores);
        }
    }
}

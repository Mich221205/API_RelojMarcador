using System.Text.Json;
using API_MarcasInconsistencias.Entities;
using API_MarcasInconsistencias.Repository;

namespace API_MarcasInconsistencias.Services
{
    public class Usr6Service : IUsr6Service
    {
        private readonly IUsr6Repository _repo;

        public Usr6Service(IUsr6Repository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<MotivoAusencia>> ObtenerMotivosAsync()
            => _repo.ObtenerMotivosAsync();

        public async Task<(bool ok, List<string> errores)> CrearPermisoAsync(PermisoCrearDto dto, string? adjuntoUrl)
        {
            var errores = new List<string>();

            if (dto.IdUsuario <= 0)
                errores.Add("Usuario inválido.");

            if (dto.IdMotivo <= 0)
                errores.Add("Seleccione un motivo.");

            if (dto.Observaciones is { Length: > 300 })
                errores.Add("Las observaciones no pueden exceder 300 caracteres.");

            if (dto.FechaHoraInicio == default || dto.FechaHoraFin == default)
                errores.Add("Debe indicar fecha y hora de inicio y fin.");
            else if (dto.FechaHoraFin <= dto.FechaHoraInicio)
                errores.Add("La fecha/hora fin debe ser posterior al inicio.");

            if (errores.Count > 0)
                return (false, errores);

            var solapaVacaciones = await _repo.TieneVacacionesSuperpuestasAsync(
                dto.IdUsuario,
                dto.FechaHoraInicio,
                dto.FechaHoraFin
            );

            if (solapaVacaciones)
            {
                errores.Add("El rango indicado se superpone con vacaciones aprobadas.");
                return (false, errores);
            }

            var payload = new
            {
                Modulo = "USR6",
                Accion = "NUEVO_PERMISO",
                Rango = $"{dto.FechaHoraInicio:yyyy-MM-dd HH:mm} -> {dto.FechaHoraFin:yyyy-MM-dd HH:mm}",
                Motivo = dto.IdMotivo,
                Adjunto = !string.IsNullOrEmpty(adjuntoUrl)
            };

            var json = JsonSerializer.Serialize(
                payload,
                new JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });

            await _repo.CrearPermisoAsync(dto, adjuntoUrl, json);

            return (true, errores);
        }

        public Task<IEnumerable<PermisoListDto>> ObtenerPermisosAsync(PermisoFiltroDto filtro)
            => _repo.ObtenerPermisosAsync(filtro);
    }
}

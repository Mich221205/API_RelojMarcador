using MarcasInconsistencias.Entities;
using MarcasInconsistencias.Repository;
using System.Text.Json;

namespace MarcasInconsistencias.Services
{
    public class InconsistenciasService : IInconsistenciasService
    {
        private readonly InconsistenciasRepository _repo;
        private readonly BitacoraRepository _bitacora;

        public InconsistenciasService(
            InconsistenciasRepository repo,
            BitacoraRepository bitacora)
        {
            _repo = repo;
            _bitacora = bitacora;
        }

        public async Task<IEnumerable<InconsistenciaReporte>> GetAllAsync(
            int idUsuarioAccion)
        {
            var data = await _repo.GetAllAsync();

            await _bitacora.RegistrarConsultaAsync(
                idUsuarioAccion,
                "El usuario consulta TODAS las inconsistencias");

            return data;
        }
    }
}

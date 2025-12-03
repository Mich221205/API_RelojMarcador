using MarcasInconsistencias.Entities;
using MarcasInconsistencias.Repository;

namespace MarcasInconsistencias.Services
{
    public class MarcaService : IMarcaService
    {
        private readonly MarcaRepository _repo;
        private readonly BitacoraRepository _bitacora;

        public MarcaService(MarcaRepository repo, BitacoraRepository bitacora)
        {
            _repo = repo;
            _bitacora = bitacora;
        }

        public async Task<IEnumerable<Marca>> GetByUsuarioAsync(int idUsuario, int idUsuarioAccion)
        {
            var marcas = await _repo.GetByUsuarioAsync(idUsuario);

            await _bitacora.RegistrarConsultaAsync(idUsuarioAccion, "El usuario consulta marcas");

            return marcas;
        }
    }
}

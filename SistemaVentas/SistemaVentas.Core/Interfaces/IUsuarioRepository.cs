using SistemaVentas.Core.DTOs;
using SistemaVentas.Core.Entities;

namespace SistemaVentas.Core.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario> ValidarUsuarioAsync(string nombreUsuario, string contrasena);
        Task<IEnumerable<Usuario>> ObtenerTodosAsync();
        Task<Usuario> CrearAsync(Usuario usuario);
    }
}

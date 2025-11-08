using SistemaVentas.Core.DTOs;
using SistemaVentas.Core.Entities;
using SistemaVentas.Core.Interfaces;
using System.Text;

namespace SistemaVentas.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public AuthService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<UsuarioDTO> LoginAsync(LoginDTO loginDto)
        {
            try
            {
                var usuario = await _usuarioRepository.ValidarUsuarioAsync(
                    loginDto.NombreUsuario,
                    loginDto.Contrasena);

                if (usuario == null)
                {
                    throw new UnauthorizedAccessException("Usuario o contraseña incorrectos");
                }

                if (!usuario.Activo)
                {
                    throw new UnauthorizedAccessException("Usuario inactivo");
                }

                return MapearUsuarioADTO(usuario);
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en el login: {ex.Message}", ex);
            }
        }

        public async Task<UsuarioDTO> CrearUsuarioAsync(CrearUsuarioDTO crearUsuarioDto)
        {
            try
            {
                // Validar que el usuario no exista
                var usuarioExistente = await _usuarioRepository.ObtenerTodosAsync();
                if (usuarioExistente.Any(u => u.NombreUsuario == crearUsuarioDto.NombreUsuario))
                {
                    throw new Exception("El nombre de usuario ya existe");
                }

                var usuario = new Usuario
                {
                    NombreUsuario = crearUsuarioDto.NombreUsuario,
                    Contrasena = Encoding.UTF8.GetBytes(crearUsuarioDto.Contrasena), // Se encripta en el repository
                    NombreCompleto = crearUsuarioDto.NombreCompleto,
                    Rol = crearUsuarioDto.Rol,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                };

                var usuarioCreado = await _usuarioRepository.CrearAsync(usuario);

                return MapearUsuarioADTO(usuarioCreado);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear usuario: {ex.Message}", ex);
            }
        }

        // Método auxiliar para mapear
        private UsuarioDTO MapearUsuarioADTO(Usuario usuario)
        {
            return new UsuarioDTO
            {
                IdUsuario = usuario.IdUsuario,
                NombreUsuario = usuario.NombreUsuario,
                NombreCompleto = usuario.NombreCompleto,
                Rol = usuario.Rol,
                Activo = usuario.Activo,
                FechaCreacion = usuario.FechaCreacion
            };
        }
    }
}
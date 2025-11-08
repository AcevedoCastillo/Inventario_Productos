using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SistemaVentas.Core.Entities;
using SistemaVentas.Core.Interfaces;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace SistemaVentas.API.Data.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ApplicationDbContext _context;

        public UsuarioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario> ValidarUsuarioAsync(string nombreUsuario, string contrasena)
        {
            try
            {
                var paramNombreUsuario = new SqlParameter("@NombreUsuario", nombreUsuario);
                var paramContrasena = new SqlParameter("@Contrasena", contrasena);

                // ✅ Usar consulta SQL directa con ADO.NET
                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "SP_ValidarUsuario";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@NombreUsuario", nombreUsuario));
                command.Parameters.Add(new SqlParameter("@Contrasena", contrasena));

                await _context.Database.OpenConnectionAsync();

                Usuario usuario = null;
                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    usuario = new Usuario
                    {
                        IdUsuario = reader.GetInt32(reader.GetOrdinal("IdUsuario")),
                        NombreUsuario = reader.GetString(reader.GetOrdinal("NombreUsuario")),
                        NombreCompleto = reader.GetString(reader.GetOrdinal("NombreCompleto")),
                        Rol = reader.GetString(reader.GetOrdinal("Rol")),
                        Activo = reader.GetBoolean(reader.GetOrdinal("Activo")),
                        FechaCreacion = DateTime.Now.Date,
                        Contrasena = new byte[0] 
                    };
                }

                await _context.Database.CloseConnectionAsync();

                return usuario;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al validar usuario: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Usuario>> ObtenerTodosAsync()
        {
            try
            {
                // ✅ Para este SP sí necesitamos modificarlo o usar ADO.NET también
                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "SP_ListarUsuarios";
                command.CommandType = CommandType.StoredProcedure;

                await _context.Database.OpenConnectionAsync();

                var usuarios = new List<Usuario>();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    usuarios.Add(new Usuario
                    {
                        IdUsuario = reader.GetInt32(reader.GetOrdinal("IdUsuario")),
                        NombreUsuario = reader.GetString(reader.GetOrdinal("NombreUsuario")),
                        NombreCompleto = reader.GetString(reader.GetOrdinal("NombreCompleto")),
                        Rol = reader.GetString(reader.GetOrdinal("Rol")),
                        Activo = reader.GetBoolean(reader.GetOrdinal("Activo")),
                        FechaCreacion = reader.GetDateTime(reader.GetOrdinal("FechaCreacion")),
                        Contrasena = new byte[0] // Contraseña vacía
                    });
                }

                await _context.Database.CloseConnectionAsync();

                return usuarios;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener usuarios: {ex.Message}", ex);
            }
        }

        public async Task<Usuario> CrearAsync(Usuario usuario)
        {
            try
            {
                // Encriptar contraseña
                string contrasenaTexto = Encoding.UTF8.GetString(usuario.Contrasena);

                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "SP_CrearUsuario";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@NombreUsuario", usuario.NombreUsuario));
                command.Parameters.Add(new SqlParameter("@Contrasena", contrasenaTexto));
                command.Parameters.Add(new SqlParameter("@NombreCompleto", usuario.NombreCompleto));
                command.Parameters.Add(new SqlParameter("@Rol", usuario.Rol));

                await _context.Database.OpenConnectionAsync();

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    usuario.IdUsuario = Convert.ToInt32(reader.GetValue(0));
                }

                await _context.Database.CloseConnectionAsync();

                return usuario;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear usuario: {ex.Message}", ex);
            }
        }
    }
}
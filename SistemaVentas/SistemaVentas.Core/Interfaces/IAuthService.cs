using SistemaVentas.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVentas.Core.Interfaces
{
    public interface IAuthService
    {
        Task<UsuarioDTO> LoginAsync(LoginDTO loginDto);
        Task<UsuarioDTO> CrearUsuarioAsync(CrearUsuarioDTO crearUsuarioDto);
    }
}
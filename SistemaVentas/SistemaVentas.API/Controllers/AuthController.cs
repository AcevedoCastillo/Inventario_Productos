using Microsoft.AspNetCore.Mvc;
using SistemaVentas.Core.DTOs;
using SistemaVentas.Core.Interfaces;

namespace SistemaVentas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Iniciar sesión
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Datos inválidos",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                var usuario = await _authService.LoginAsync(loginDto);

                return Ok(new
                {
                    Success = true,
                    Message = "Login exitoso",
                    Data = usuario
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Crear nuevo usuario
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CrearUsuarioDTO crearUsuarioDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Datos inválidos",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                var usuario = await _authService.CrearUsuarioAsync(crearUsuarioDto);

                return Ok(new
                {
                    Success = true,
                    Message = "Usuario creado exitosamente",
                    Data = usuario
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }
    }
}
using System.ComponentModel.DataAnnotations;

namespace SistemaVentas.Core.DTOs
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        public string NombreUsuario { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        public string Contrasena { get; set; }
    }
}

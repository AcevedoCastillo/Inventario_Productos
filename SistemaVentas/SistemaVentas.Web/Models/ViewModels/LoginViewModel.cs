using System.ComponentModel.DataAnnotations;

namespace SistemaVentas.Web.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El usuario es requerido")]
        [Display(Name = "Usuario")]
        public string NombreUsuario { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; }
    }

    public class UsuarioSesion
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string NombreCompleto { get; set; }
        public string Rol { get; set; }
    }
}
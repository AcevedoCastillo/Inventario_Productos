namespace SistemaVentas.Core.DTOs
{
    public class UsuarioDTO
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string NombreCompleto { get; set; }
        public string Rol { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

    public class CrearUsuarioDTO
    {
        public string NombreUsuario { get; set; }
        public string Contrasena { get; set; }
        public string NombreCompleto { get; set; }
        public string Rol { get; set; }
    }
}

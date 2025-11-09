using System.ComponentModel.DataAnnotations;

namespace SistemaVentas.Core.DTOs
{
    public class ProductoDTO
    {
        public int IdPro { get; set; }

        [Required(ErrorMessage = "El código es requerido")]
        [StringLength(20)]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "El nombre del producto es requerido")]
        [StringLength(100)]
        public string Producto { get; set; }

        [Required(ErrorMessage = "El precio es requerido")]
        [Range(0.01, 999999.99, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal Precio { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int Stock { get; set; }

        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}

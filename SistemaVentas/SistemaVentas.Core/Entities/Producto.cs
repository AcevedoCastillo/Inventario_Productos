using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaVentas.Core.Entities
{
    [Table("Productos")]
    public class Producto
    {
        [Key]
        public int IdPro { get; set; }

        [Required]
        [StringLength(20)]
        public string Codigo { get; set; }

        [Required]
        [StringLength(100)]
        public string Producto1 { get; set; } // Producto es palabra reservada en algunos contextos

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        public int Stock { get; set; } = 0;

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Navegación
        public virtual ICollection<DetalleVenta> DetalleVentas { get; set; }

        [NotMapped]
        public string NombreProducto
        {
            get => Producto1;
            set => Producto1 = value;
        }
    }
}
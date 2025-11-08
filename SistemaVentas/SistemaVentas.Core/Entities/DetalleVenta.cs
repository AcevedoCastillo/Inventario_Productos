using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaVentas.Core.Entities
{
    [Table("DetalleVentas")]
    public class DetalleVenta
    {
        [Key]
        public int IdDe { get; set; }

        [Required]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required]
        public int IdVenta { get; set; }

        [Required]
        public int IdPro { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal IVA { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Total { get; set; }

        // Navegación
        [ForeignKey("IdVenta")]
        public virtual Venta Venta { get; set; }

        [ForeignKey("IdPro")]
        public virtual Producto Producto { get; set; }
    }
}
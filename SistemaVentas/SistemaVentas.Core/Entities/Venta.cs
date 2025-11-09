using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaVentas.Core.Entities
{
    [Table("Ventas")]
    public class Venta
    {
        [Key]
        public int IdVenta { get; set; }

        [Required]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required]
        [StringLength(100)]
        public string Vendedor { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal SubTotal { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalIVA { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Total { get; set; }

        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }


        // Navegación
        [ForeignKey("IdUsuario")]
        public virtual Usuario Usuario { get; set; }

        public virtual ICollection<DetalleVenta> DetalleVentas { get; set; }
    }
}

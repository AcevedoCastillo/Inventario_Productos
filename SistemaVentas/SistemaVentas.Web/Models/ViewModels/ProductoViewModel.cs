using System.ComponentModel.DataAnnotations;

namespace SistemaVentas.Web.Models.ViewModels
{
    public class ProductoViewModel
    {
        public int IdPro { get; set; }

        [Required(ErrorMessage = "El código es requerido")]
        [Display(Name = "Código")]
        [StringLength(20)]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "El nombre del producto es requerido")]
        [Display(Name = "Producto")]
        [StringLength(100)]
        public string Producto { get; set; }

        [Required(ErrorMessage = "El precio es requerido")]
        [Display(Name = "Precio")]
        [Range(0.01, 999999.99, ErrorMessage = "El precio debe ser mayor a 0")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "El stock es requerido")]
        [Display(Name = "Stock")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int Stock { get; set; }

        public bool Activo { get; set; }

        [Display(Name = "Fecha de Creación")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaCreacion { get; set; }
    }
}
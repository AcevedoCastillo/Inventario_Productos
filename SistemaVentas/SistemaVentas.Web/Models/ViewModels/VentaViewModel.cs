using System.ComponentModel.DataAnnotations;

namespace SistemaVentas.Web.Models.ViewModels
{
    public class VentaViewModel
    {
        public int IdVenta { get; set; }

        [Display(Name = "Fecha")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime Fecha { get; set; }

        [Display(Name = "Vendedor")]
        public string Vendedor { get; set; }

        [Display(Name = "Subtotal")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal SubTotal { get; set; }

        [Display(Name = "IVA")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal TotalIVA { get; set; }

        [Display(Name = "Total")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Total { get; set; }

        public string NombreUsuario { get; set; }
        public List<DetalleVentaViewModel> Detalles { get; set; }
    }

    public class DetalleVentaViewModel
    {
        public int IdDe { get; set; }
        public string Codigo { get; set; }

        [Display(Name = "Producto")]
        public string Producto { get; set; }

        [Display(Name = "Cantidad")]
        public int Cantidad { get; set; }

        [Display(Name = "Precio")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Precio { get; set; }

        [Display(Name = "IVA")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal IVA { get; set; }

        [Display(Name = "Total")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Total { get; set; }
    }

    public class CrearVentaViewModel
    {
        [Required(ErrorMessage = "El vendedor es requerido")]
        [Display(Name = "Vendedor")]
        public string Vendedor { get; set; }

        public int IdUsuario { get; set; }

        public List<ItemVentaViewModel> Detalles { get; set; } = new List<ItemVentaViewModel>();
    }

    public class ItemVentaViewModel
    {
        public int IdPro { get; set; }
        public string Codigo { get; set; }
        public string Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
    }
}
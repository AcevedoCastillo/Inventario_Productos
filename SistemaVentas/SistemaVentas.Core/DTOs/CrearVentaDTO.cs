using System.ComponentModel.DataAnnotations;

namespace SistemaVentas.Core.DTOs
{
    public class CrearVentaDTO
    {
        [Required]
        public string Vendedor { get; set; }

        [Required]
        public int IdUsuario { get; set; }

        [Required]
        public List<ItemVentaDTO> Detalles { get; set; }
    }

    public class ItemVentaDTO
    {
        [Required]
        public int IdPro { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }

        [Required]
        public decimal Precio { get; set; }

        [Required]
        public decimal IVA { get; set; }

        [Required]
        public decimal Total { get; set; }
    }
}

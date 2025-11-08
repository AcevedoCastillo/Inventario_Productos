namespace SistemaVentas.Core.DTOs
{
    public class VentaDTO
    {
        public int IdVenta { get; set; }
        public DateTime Fecha { get; set; }
        public string Vendedor { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TotalIVA { get; set; }
        public decimal Total { get; set; }
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public List<DetalleVentaDTO> Detalles { get; set; }
    }
}

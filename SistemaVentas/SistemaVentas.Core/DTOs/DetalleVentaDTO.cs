namespace SistemaVentas.Core.DTOs
{
    public class DetalleVentaDTO
    {
        public int IdDe { get; set; }
        public DateTime Fecha { get; set; }
        public int IdVenta { get; set; }
        public int IdPro { get; set; }
        public string Codigo { get; set; }
        public string Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
    }
}

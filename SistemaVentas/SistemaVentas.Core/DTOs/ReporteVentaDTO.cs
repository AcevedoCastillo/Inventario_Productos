namespace SistemaVentas.Core.DTOs
{
    public class ReporteVentaDTO
    {
        public int NoVenta { get; set; }
        public string Fecha { get; set; }
        public string Vendedor { get; set; }
        public int CantidadProductos { get; set; }
        public decimal SubTotal { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
    }

    public class ReporteDetalladoVentaDTO
    {
        public int NoVenta { get; set; }
        public string Fecha { get; set; }
        public string Vendedor { get; set; }
        public string Codigo { get; set; }
        public string Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal IVA { get; set; }
        public decimal TotalProducto { get; set; }
        public decimal TotalVenta { get; set; }
    }
}

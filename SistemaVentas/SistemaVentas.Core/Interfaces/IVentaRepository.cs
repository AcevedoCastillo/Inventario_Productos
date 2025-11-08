using SistemaVentas.Core.DTOs;
using SistemaVentas.Core.Entities;

namespace SistemaVentas.Core.Interfaces
{
    public interface IVentaRepository
    {
        Task<int> CrearVentaAsync(Venta venta, List<DetalleVenta> detalles);
        Task<IEnumerable<Venta>> ObtenerVentasAsync(DateTime? fechaInicio, DateTime? fechaFin);
        Task<Venta> ObtenerVentaPorIdAsync(int id);
        Task<IEnumerable<ReporteVentaDTO>> ObtenerReporteVentasAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<IEnumerable<ReporteDetalladoVentaDTO>> ObtenerReporteDetalladoAsync(DateTime fechaInicio, DateTime fechaFin);
    }
}

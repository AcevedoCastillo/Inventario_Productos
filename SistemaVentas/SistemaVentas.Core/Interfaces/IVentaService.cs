using SistemaVentas.Core.DTOs;

namespace SistemaVentas.Core.Interfaces
{
    public interface IVentaService
    {
        Task<int> CrearVentaAsync(CrearVentaDTO crearVentaDto);
        Task<IEnumerable<VentaDTO>> ObtenerVentasAsync(DateTime? fechaInicio, DateTime? fechaFin);
        Task<VentaDTO> ObtenerVentaPorIdAsync(int id);
        Task<IEnumerable<ReporteVentaDTO>> ObtenerReporteVentasAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<IEnumerable<ReporteDetalladoVentaDTO>> ObtenerReporteDetalladoAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<byte[]> GenerarReportePDFAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<byte[]> GenerarReporteExcelAsync(DateTime fechaInicio, DateTime fechaFin);
    }
}
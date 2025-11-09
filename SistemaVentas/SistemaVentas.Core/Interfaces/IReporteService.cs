using SistemaVentas.Core.DTOs;

namespace SistemaVentas.Core.Interfaces
{
    public interface IReporteService
    {
        Task<byte[]> GenerarReportePDFAsync(List<ReporteDetalladoVentaDTO> ventas, DateTime fechaInicio, DateTime fechaFin);
        Task<byte[]> GenerarReporteExcelAsync(List<ReporteDetalladoVentaDTO> ventas, DateTime fechaInicio, DateTime fechaFin);
    }
}
using SistemaVentas.Web.Models.ViewModels;

namespace SistemaVentas.Web.Services.Interfaces
{
    public interface IVentaApiService
    {
        Task<int> CrearVentaAsync(CrearVentaViewModel venta);
        Task<List<VentaViewModel>> ObtenerVentasAsync(DateTime? fechaInicio, DateTime? fechaFin);
        Task<VentaViewModel> ObtenerVentaPorIdAsync(int id);
        Task<byte[]> DescargarReportePDFAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<byte[]> DescargarReporteExcelAsync(DateTime fechaInicio, DateTime fechaFin);
    }
}
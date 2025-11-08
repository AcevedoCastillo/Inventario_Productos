using Newtonsoft.Json.Linq;
using SistemaVentas.Web.Models.ViewModels;
using SistemaVentas.Web.Services.Interfaces;

namespace SistemaVentas.Web.Services.Implementation
{
    public class VentaApiService : IVentaApiService
    {
        private readonly IApiService _apiService;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public VentaApiService(IApiService apiService, HttpClient httpClient, IConfiguration configuration)
        {
            _apiService = apiService;
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]);
        }

        public async Task<int> CrearVentaAsync(CrearVentaViewModel venta)
        {
            // Mapear a DTO de la API
            var ventaDto = new
            {
                vendedor = venta.Vendedor,
                idUsuario = venta.IdUsuario,
                detalles = venta.Detalles.Select(d => new
                {
                    idPro = d.IdPro,
                    cantidad = d.Cantidad,
                    precio = d.Precio,
                    iva = d.IVA,
                    total = d.Total
                }).ToList()
            };

            var response = await _apiService.PostAsync<JObject>("/ventas", ventaDto);
            return response["data"]["idVenta"].ToObject<int>();
        }

        public async Task<List<VentaViewModel>> ObtenerVentasAsync(DateTime? fechaInicio, DateTime? fechaFin)
        {
            var endpoint = "/ventas";

            if (fechaInicio.HasValue && fechaFin.HasValue)
            {
                endpoint += $"?fechaInicio={fechaInicio.Value:yyyy-MM-dd}&fechaFin={fechaFin.Value:yyyy-MM-dd}";
            }

            var response = await _apiService.GetAsync<JObject>(endpoint);
            var ventas = response["data"].ToObject<List<VentaViewModel>>();
            return ventas;
        }

        public async Task<VentaViewModel> ObtenerVentaPorIdAsync(int id)
        {
            var response = await _apiService.GetAsync<JObject>($"/ventas/{id}");
            var venta = response["data"].ToObject<VentaViewModel>();
            return venta;
        }

        public async Task<byte[]> DescargarReportePDFAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var endpoint = $"/ventas/reporte-pdf?fechaInicio={fechaInicio:yyyy-MM-dd}&fechaFin={fechaFin:yyyy-MM-dd}";
            var response = await _httpClient.GetAsync(endpoint);
            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task<byte[]> DescargarReporteExcelAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var endpoint = $"/ventas/reporte-excel?fechaInicio={fechaInicio:yyyy-MM-dd}&fechaFin={fechaFin:yyyy-MM-dd}";
            var response = await _httpClient.GetAsync(endpoint);
            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}
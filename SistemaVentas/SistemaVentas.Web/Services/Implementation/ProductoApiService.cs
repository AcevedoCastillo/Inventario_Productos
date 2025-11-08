using Newtonsoft.Json.Linq;
using SistemaVentas.Web.Models.ViewModels;
using SistemaVentas.Web.Services.Interfaces;

namespace SistemaVentas.Web.Services.Implementation
{
    public class ProductoApiService : IProductoApiService
    {
        private readonly IApiService _apiService;

        public ProductoApiService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<List<ProductoViewModel>> ObtenerTodosAsync()
        {
            var response = await _apiService.GetAsync<JObject>("/productos");
            var productos = response["data"].ToObject<List<ProductoViewModel>>();
            return productos;
        }

        public async Task<ProductoViewModel> ObtenerPorIdAsync(int id)
        {
            var response = await _apiService.GetAsync<JObject>($"/productos/{id}");
            var producto = response["data"].ToObject<ProductoViewModel>();
            return producto;
        }

        public async Task<ProductoViewModel> ObtenerPorCodigoAsync(string codigo)
        {
            var response = await _apiService.GetAsync<JObject>($"/productos/buscar/{codigo}");
            var producto = response["data"].ToObject<ProductoViewModel>();
            return producto;
        }

        public async Task<ProductoViewModel> CrearAsync(ProductoViewModel producto)
        {
            var response = await _apiService.PostAsync<JObject>("/productos", producto);
            var productoCreado = response["data"].ToObject<ProductoViewModel>();
            return productoCreado;
        }

        public async Task<bool> ActualizarAsync(int id, ProductoViewModel producto)
        {
            var response = await _apiService.PutAsync<JObject>($"/productos/{id}", producto);
            return response["success"].ToObject<bool>();
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var response = await _apiService.DeleteAsync<JObject>($"/productos/{id}");
            return response["success"].ToObject<bool>();
        }
    }
}
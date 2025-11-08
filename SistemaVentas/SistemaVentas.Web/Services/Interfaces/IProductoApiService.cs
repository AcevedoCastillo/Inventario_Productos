using SistemaVentas.Web.Models.ViewModels;

namespace SistemaVentas.Web.Services.Interfaces
{
    public interface IProductoApiService
    {
        Task<List<ProductoViewModel>> ObtenerTodosAsync();
        Task<ProductoViewModel> ObtenerPorIdAsync(int id);
        Task<ProductoViewModel> ObtenerPorCodigoAsync(string codigo);
        Task<ProductoViewModel> CrearAsync(ProductoViewModel producto);
        Task<bool> ActualizarAsync(int id, ProductoViewModel producto);
        Task<bool> EliminarAsync(int id);
    }
}
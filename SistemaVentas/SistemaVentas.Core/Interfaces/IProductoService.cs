using SistemaVentas.Core.DTOs;

namespace SistemaVentas.Core.Interfaces
{
    public interface IProductoService
    {
        Task<IEnumerable<ProductoDTO>> ObtenerTodosAsync();
        Task<ProductoDTO> ObtenerPorIdAsync(int id);
        Task<ProductoDTO> ObtenerPorCodigoAsync(string codigo);
        Task<ProductoDTO> CrearAsync(ProductoDTO productoDto);
        Task<bool> ActualizarAsync(int id, ProductoDTO productoDto);
        Task<bool> EliminarAsync(int id);
        Task<bool> VerificarStockAsync(int idPro, int cantidad);
    }
}
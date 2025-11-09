using SistemaVentas.Core.Entities;

namespace SistemaVentas.Core.Interfaces
{
    public interface IProductoRepository
    {
        Task<IEnumerable<Producto>> ObtenerTodosAsync();
        Task<Producto> ObtenerPorIdAsync(int id);
        Task<Producto> ObtenerPorCodigoAsync(string codigo);
        Task<Producto> CrearAsync(Producto producto);
        Task<bool> ActualizarAsync(Producto producto);
        Task<bool> EliminarAsync(int id);
        Task<bool> ExisteCodigoAsync(string codigo, int? idExcluir = null);
        Task<bool> VerificarStockAsync(int idPro, int cantidad);
    }
}

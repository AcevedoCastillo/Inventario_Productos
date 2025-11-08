using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SistemaVentas.Core.Entities;
using SistemaVentas.Core.Interfaces;

namespace SistemaVentas.API.Data.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Producto>> ObtenerTodosAsync()
        {
            try
            {
                var productos = await _context.Productos
                    .FromSqlRaw("EXEC SP_ListarProductos")
                    .AsNoTracking()
                    .ToListAsync();

                return productos;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener productos: {ex.Message}", ex);
            }
        }

        public async Task<Producto> ObtenerPorIdAsync(int id)
        {
            try
            {
                var paramId = new SqlParameter("@IdPro", id);

                var producto = await _context.Productos
                    .FromSqlRaw("EXEC SP_ObtenerProductoPorId @IdPro", paramId)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                return producto;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener producto: {ex.Message}", ex);
            }
        }

        public async Task<Producto> ObtenerPorCodigoAsync(string codigo)
        {
            try
            {
                var paramCodigo = new SqlParameter("@Codigo", codigo);

                var producto = await _context.Productos
                    .FromSqlRaw("EXEC SP_BuscarProductoPorCodigo @Codigo", paramCodigo)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                return producto;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al buscar producto por código: {ex.Message}", ex);
            }
        }

        public async Task<Producto> CrearAsync(Producto producto)
        {
            try
            {
                var paramCodigo = new SqlParameter("@Codigo", producto.Codigo);
                var paramProducto = new SqlParameter("@Producto", producto.NombreProducto);
                var paramPrecio = new SqlParameter("@Precio", producto.Precio);
                var paramStock = new SqlParameter("@Stock", producto.Stock);

                var result = await _context.Database
                    .SqlQueryRaw<int>("EXEC SP_CrearProducto @Codigo, @Producto, @Precio, @Stock",
                        paramCodigo, paramProducto, paramPrecio, paramStock)
                    .ToListAsync();

                if (result.Any())
                {
                    producto.IdPro = result.First();
                    return producto;
                }

                return null;
            }
            catch (SqlException ex) when (ex.Message.Contains("ya existe"))
            {
                throw new Exception("El código de producto ya existe.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear producto: {ex.Message}", ex);
            }
        }

        public async Task<bool> ActualizarAsync(Producto producto)
        {
            try
            {
                var paramIdPro = new SqlParameter("@IdPro", producto.IdPro);
                var paramCodigo = new SqlParameter("@Codigo", producto.Codigo);
                var paramProducto = new SqlParameter("@Producto", producto.NombreProducto);
                var paramPrecio = new SqlParameter("@Precio", producto.Precio);
                var paramStock = new SqlParameter("@Stock", producto.Stock);

                var result = await _context.Database
                    .ExecuteSqlRawAsync(
                        "EXEC SP_ActualizarProducto @IdPro, @Codigo, @Producto, @Precio, @Stock",
                        paramIdPro, paramCodigo, paramProducto, paramPrecio, paramStock);

                return result > 0;
            }
            catch (SqlException ex) when (ex.Message.Contains("ya existe"))
            {
                throw new Exception("El código de producto ya existe en otro registro.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar producto: {ex.Message}", ex);
            }
        }

        public async Task<bool> EliminarAsync(int id)
        {
            try
            {
                var paramId = new SqlParameter("@IdPro", id);

                var result = await _context.Database
                    .ExecuteSqlRawAsync("EXEC SP_EliminarProducto @IdPro", paramId);

                return result > 0;
            }
            catch (SqlException ex) when (ex.Message.Contains("ventas asociadas"))
            {
                throw new Exception("No se puede eliminar el producto porque tiene ventas asociadas.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar producto: {ex.Message}", ex);
            }
        }

        public async Task<bool> ExisteCodigoAsync(string codigo, int? idExcluir = null)
        {
            try
            {
                var query = _context.Productos
                    .Where(p => p.Codigo == codigo && p.Activo);

                if (idExcluir.HasValue)
                {
                    query = query.Where(p => p.IdPro != idExcluir.Value);
                }

                return await query.AnyAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al verificar código: {ex.Message}", ex);
            }
        }

        public async Task<bool> VerificarStockAsync(int idPro, int cantidad)
        {
            try
            {
                var paramIdPro = new SqlParameter("@IdPro", idPro);
                var paramCantidad = new SqlParameter("@Cantidad", cantidad);

                // Crear clase para resultado del SP
                var result = await _context.Database
                    .SqlQueryRaw<StockVerificacionResult>(
                        "EXEC SP_VerificarStock @IdPro, @Cantidad",
                        paramIdPro, paramCantidad)
                    .FirstOrDefaultAsync();

                return result?.Disponible == 1;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al verificar stock: {ex.Message}", ex);
            }
        }
    }

    // Clase auxiliar para el resultado del SP_VerificarStock
    public class StockVerificacionResult
    {
        public int Disponible { get; set; }
        public int? StockActual { get; set; }
        public string Mensaje { get; set; }
    }
}
using SistemaVentas.Core.DTOs;
using SistemaVentas.Core.Entities;
using SistemaVentas.Core.Interfaces;

namespace SistemaVentas.API.Services
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _productoRepository;

        public ProductoService(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        public async Task<IEnumerable<ProductoDTO>> ObtenerTodosAsync()
        {
            try
            {
                var productos = await _productoRepository.ObtenerTodosAsync();
                return productos.Select(MapearProductoADTO);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener productos: {ex.Message}", ex);
            }
        }

        public async Task<ProductoDTO> ObtenerPorIdAsync(int id)
        {
            try
            {
                var producto = await _productoRepository.ObtenerPorIdAsync(id);

                if (producto == null)
                {
                    throw new KeyNotFoundException($"Producto con ID {id} no encontrado");
                }

                return MapearProductoADTO(producto);
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener producto: {ex.Message}", ex);
            }
        }

        public async Task<ProductoDTO> ObtenerPorCodigoAsync(string codigo)
        {
            try
            {
                var producto = await _productoRepository.ObtenerPorCodigoAsync(codigo);

                if (producto == null)
                {
                    throw new KeyNotFoundException($"Producto con código {codigo} no encontrado");
                }

                return MapearProductoADTO(producto);
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al buscar producto: {ex.Message}", ex);
            }
        }

        public async Task<ProductoDTO> CrearAsync(ProductoDTO productoDto)
        {
            try
            {
                // Validar que el código no exista
                var existe = await _productoRepository.ExisteCodigoAsync(productoDto.Codigo);
                if (existe)
                {
                    throw new Exception($"Ya existe un producto con el código {productoDto.Codigo}");
                }

                var producto = new Producto
                {
                    Codigo = productoDto.Codigo,
                    NombreProducto = productoDto.Producto,
                    Precio = productoDto.Precio,
                    Stock = productoDto.Stock,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                };

                var productoCreado = await _productoRepository.CrearAsync(producto);

                if (productoCreado == null)
                {
                    throw new Exception("Error al crear el producto");
                }

                return MapearProductoADTO(productoCreado);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear producto: {ex.Message}", ex);
            }
        }

        public async Task<bool> ActualizarAsync(int id, ProductoDTO productoDto)
        {
            try
            {
                // Verificar que el producto existe
                var productoExistente = await _productoRepository.ObtenerPorIdAsync(id);
                if (productoExistente == null)
                {
                    throw new KeyNotFoundException($"Producto con ID {id} no encontrado");
                }

                // Validar que el código no exista en otro producto
                var existe = await _productoRepository.ExisteCodigoAsync(productoDto.Codigo, id);
                if (existe)
                {
                    throw new Exception($"Ya existe otro producto con el código {productoDto.Codigo}");
                }

                var producto = new Producto
                {
                    IdPro = id,
                    Codigo = productoDto.Codigo,
                    NombreProducto = productoDto.Producto,
                    Precio = productoDto.Precio,
                    Stock = productoDto.Stock,
                    Activo = productoDto.Activo,
                    FechaCreacion = productoExistente.FechaCreacion
                };

                return await _productoRepository.ActualizarAsync(producto);
            }
            catch (KeyNotFoundException)
            {
                throw;
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
                // Verificar que el producto existe
                var producto = await _productoRepository.ObtenerPorIdAsync(id);
                if (producto == null)
                {
                    throw new KeyNotFoundException($"Producto con ID {id} no encontrado");
                }

                return await _productoRepository.EliminarAsync(id);
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar producto: {ex.Message}", ex);
            }
        }

        public async Task<bool> VerificarStockAsync(int idPro, int cantidad)
        {
            try
            {
                return await _productoRepository.VerificarStockAsync(idPro, cantidad);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al verificar stock: {ex.Message}", ex);
            }
        }

        // Método auxiliar para mapear
        private ProductoDTO MapearProductoADTO(Producto producto)
        {
            return new ProductoDTO
            {
                IdPro = producto.IdPro,
                Codigo = producto.Codigo,
                Producto = producto.NombreProducto,
                Precio = producto.Precio,
                Stock = producto.Stock,
                Activo = producto.Activo,
                FechaCreacion = producto.FechaCreacion
            };
        }
    }
}

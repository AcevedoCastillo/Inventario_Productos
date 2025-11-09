using SistemaVentas.Core.DTOs;
using SistemaVentas.Core.Entities;
using SistemaVentas.Core.Interfaces;

namespace SistemaVentas.API.Services
{
    public class VentaService : IVentaService
    {
        private readonly IVentaRepository _ventaRepository;
        private readonly IProductoRepository _productoRepository;
        private const decimal PORCENTAJE_IVA = 0.13m; // 13% IVA

        public VentaService(IVentaRepository ventaRepository, IProductoRepository productoRepository)
        {
            _ventaRepository = ventaRepository;
            _productoRepository = productoRepository;
        }

        public async Task<int> CrearVentaAsync(CrearVentaDTO crearVentaDto)
        {
            try
            {
                // Validar que hay detalles
                if (crearVentaDto.Detalles == null || !crearVentaDto.Detalles.Any())
                {
                    throw new Exception("La venta debe tener al menos un producto");
                }

                // Verificar stock de todos los productos
                foreach (var detalle in crearVentaDto.Detalles)
                {
                    var tieneStock = await _productoRepository.VerificarStockAsync(
                        detalle.IdPro,
                        detalle.Cantidad);

                    if (!tieneStock)
                    {
                        var producto = await _productoRepository.ObtenerPorIdAsync(detalle.IdPro);
                        throw new Exception($"Stock insuficiente para el producto: {producto.NombreProducto}");
                    }
                }

                // Calcular totales
                decimal subTotal = crearVentaDto.Detalles.Sum(d => d.Precio * d.Cantidad);
                decimal totalIVA = subTotal * PORCENTAJE_IVA;
                decimal total = subTotal + totalIVA;

                // Crear entidad Venta
                var venta = new Venta
                {
                    Fecha = DateTime.Now,
                    Vendedor = crearVentaDto.Vendedor,
                    SubTotal = subTotal,
                    TotalIVA = totalIVA,
                    Total = total,
                    IdUsuario = crearVentaDto.IdUsuario
                };

                // Crear detalles
                var detalles = crearVentaDto.Detalles.Select(d => new DetalleVenta
                {
                    Fecha = DateTime.Now,
                    IdPro = d.IdPro,
                    Cantidad = d.Cantidad,
                    Precio = d.Precio,
                    IVA = d.Precio * d.Cantidad * PORCENTAJE_IVA,
                    Total = (d.Precio * d.Cantidad) + (d.Precio * d.Cantidad * PORCENTAJE_IVA)
                }).ToList();

                // Guardar venta
                var idVenta = await _ventaRepository.CrearVentaAsync(venta, detalles);

                if (idVenta == 0)
                {
                    throw new Exception("Error al crear la venta");
                }

                return idVenta;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear venta: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<VentaDTO>> ObtenerVentasAsync(DateTime? fechaInicio, DateTime? fechaFin)
        {
            try
            {
                var ventas = await _ventaRepository.ObtenerVentasAsync(fechaInicio, fechaFin);
                return ventas.Select(MapearVentaADTO);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener ventas: {ex.Message}", ex);
            }
        }

        public async Task<VentaDTO> ObtenerVentaPorIdAsync(int id)
        {
            try
            {
                var venta = await _ventaRepository.ObtenerVentaPorIdAsync(id);

                if (venta == null)
                {
                    throw new KeyNotFoundException($"Venta con ID {id} no encontrada");
                }

                return MapearVentaADTO(venta);
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener venta: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<ReporteVentaDTO>> ObtenerReporteVentasAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                return await _ventaRepository.ObtenerReporteVentasAsync(fechaInicio, fechaFin);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al generar reporte: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<ReporteDetalladoVentaDTO>> ObtenerReporteDetalladoAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                return await _ventaRepository.ObtenerReporteDetalladoAsync(fechaInicio, fechaFin);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al generar reporte detallado: {ex.Message}", ex);
            }
        }

        public async Task<byte[]> GenerarReportePDFAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            // Implementaremos después
            throw new NotImplementedException("Generación de PDF pendiente");
        }

        public async Task<byte[]> GenerarReporteExcelAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            // Implementaremos después
            throw new NotImplementedException("Generación de Excel pendiente");
        }

        // Método auxiliar para mapear
        private VentaDTO MapearVentaADTO(Venta venta)
        {
            return new VentaDTO
            {
                IdVenta = venta.IdVenta,
                Fecha = venta.Fecha,
                Vendedor = venta.Vendedor,
                SubTotal = venta.SubTotal,
                TotalIVA = venta.TotalIVA,
                Total = venta.Total,
                IdUsuario = venta.IdUsuario,
                NombreUsuario = venta.NombreUsuario,
                Detalles = venta.DetalleVentas?.Select(d => new DetalleVentaDTO
                {
                    IdDe = d.IdDe,
                    Fecha = d.Fecha,
                    IdVenta = d.IdVenta,
                    IdPro = d.IdPro,
                    Codigo = d.Producto?.Codigo,
                    Producto = d.Producto?.NombreProducto,
                    Cantidad = d.Cantidad,
                    Precio = d.Precio,
                    IVA = d.IVA,
                    Total = d.Total
                }).ToList()
            };
        }
    }
}
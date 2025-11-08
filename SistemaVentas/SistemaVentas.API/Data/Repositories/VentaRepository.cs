using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SistemaVentas.Core.DTOs;
using SistemaVentas.Core.Entities;
using SistemaVentas.Core.Interfaces;
using System.Data;
using System.Text.Json;

namespace SistemaVentas.API.Data.Repositories
{
    public class VentaRepository : IVentaRepository
    {
        private readonly ApplicationDbContext _context;

        public VentaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> CrearVentaAsync(Venta venta, List<DetalleVenta> detalles)
        {
            try
            {
                // Convertir detalles a JSON
                var detallesJson = JsonSerializer.Serialize(detalles.Select(d => new
                {
                    IdPro = d.IdPro,
                    Cantidad = d.Cantidad,
                    Precio = d.Precio,
                    IVA = d.IVA,
                    Total = d.Total
                }));

                var paramVendedor = new SqlParameter("@Vendedor", venta.Vendedor);
                var paramSubTotal = new SqlParameter("@SubTotal", venta.SubTotal);
                var paramTotalIVA = new SqlParameter("@TotalIVA", venta.TotalIVA);
                var paramTotal = new SqlParameter("@Total", venta.Total);
                var paramIdUsuario = new SqlParameter("@IdUsuario", venta.IdUsuario);
                var paramDetalles = new SqlParameter("@DetallesVenta", SqlDbType.NVarChar, -1)
                {
                    Value = detallesJson
                };

                // Ejecutar SP y obtener el IdVenta generado
                var result = await _context.Database
                    .SqlQueryRaw<VentaCreacionResult>(
                        "EXEC SP_CrearVenta @Vendedor, @SubTotal, @TotalIVA, @Total, @IdUsuario, @DetallesVenta",
                        paramVendedor, paramSubTotal, paramTotalIVA, paramTotal, paramIdUsuario, paramDetalles)
                    .FirstOrDefaultAsync();

                return result?.IdVenta ?? 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear venta: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Venta>> ObtenerVentasAsync(DateTime? fechaInicio, DateTime? fechaFin)
        {
            try
            {
                var paramFechaInicio = fechaInicio.HasValue
                    ? new SqlParameter("@FechaInicio", fechaInicio.Value)
                    : new SqlParameter("@FechaInicio", DBNull.Value);

                var paramFechaFin = fechaFin.HasValue
                    ? new SqlParameter("@FechaFin", fechaFin.Value)
                    : new SqlParameter("@FechaFin", DBNull.Value);

                var ventas = await _context.Ventas
                    .FromSqlRaw("EXEC SP_ListarVentas @FechaInicio, @FechaFin",
                        paramFechaInicio, paramFechaFin)
                    .Include(v => v.Usuario)
                    .AsNoTracking()
                    .ToListAsync();

                return ventas;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener ventas: {ex.Message}", ex);
            }
        }

        public async Task<Venta> ObtenerVentaPorIdAsync(int id)
        {
            try
            {
                var paramId = new SqlParameter("@IdVenta", id);

                // Nota: SP_ObtenerVentaPorId retorna 2 resultados, necesitamos manejarlos por separado
                // Usaremos consultas directas con Include para este caso
                var venta = await _context.Ventas
                    .Include(v => v.Usuario)
                    .Include(v => v.DetalleVentas)
                        .ThenInclude(dv => dv.Producto)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(v => v.IdVenta == id);

                return venta;
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
                var paramFechaInicio = new SqlParameter("@FechaInicio", fechaInicio);
                var paramFechaFin = new SqlParameter("@FechaFin", fechaFin);

                var reporte = await _context.Database
                    .SqlQueryRaw<ReporteVentaDTO>(
                        "EXEC SP_ReporteVentas @FechaInicio, @FechaFin",
                        paramFechaInicio, paramFechaFin)
                    .ToListAsync();

                return reporte;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al generar reporte de ventas: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<ReporteDetalladoVentaDTO>> ObtenerReporteDetalladoAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                var paramFechaInicio = new SqlParameter("@FechaInicio", fechaInicio);
                var paramFechaFin = new SqlParameter("@FechaFin", fechaFin);

                var reporte = await _context.Database
                    .SqlQueryRaw<ReporteDetalladoVentaDTO>(
                        "EXEC SP_ReporteDetalladoVentas @FechaInicio, @FechaFin",
                        paramFechaInicio, paramFechaFin)
                    .ToListAsync();

                return reporte;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al generar reporte detallado: {ex.Message}", ex);
            }
        }
    }

    // Clase auxiliar para el resultado del SP_CrearVenta
    public class VentaCreacionResult
    {
        public int IdVenta { get; set; }
    }
}
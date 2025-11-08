using Microsoft.AspNetCore.Mvc;
using SistemaVentas.Core.DTOs;
using SistemaVentas.Core.Interfaces;

namespace SistemaVentas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentasController : ControllerBase
    {
        private readonly IVentaService _ventaService;

        public VentasController(IVentaService ventaService)
        {
            _ventaService = ventaService;
        }

        /// <summary>
        /// Crear nueva venta
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearVentaDTO crearVentaDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Datos inválidos",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                var idVenta = await _ventaService.CrearVentaAsync(crearVentaDto);

                return Ok(new
                {
                    Success = true,
                    Message = "Venta registrada exitosamente",
                    Data = new { IdVenta = idVenta }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtener todas las ventas con filtros opcionales
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] DateTime? fechaInicio, [FromQuery] DateTime? fechaFin)
        {
            try
            {
                var ventas = await _ventaService.ObtenerVentasAsync(fechaInicio, fechaFin);

                return Ok(new
                {
                    Success = true,
                    Message = "Ventas obtenidas exitosamente",
                    Data = ventas
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Error al obtener ventas",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtener venta por ID con detalle
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var venta = await _ventaService.ObtenerVentaPorIdAsync(id);

                return Ok(new
                {
                    Success = true,
                    Message = "Venta obtenida exitosamente",
                    Data = venta
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Error al obtener venta",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtener reporte de ventas por período
        /// </summary>
        [HttpGet("reporte")]
        public async Task<IActionResult> GetReporte([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            try
            {
                var reporte = await _ventaService.ObtenerReporteVentasAsync(fechaInicio, fechaFin);

                return Ok(new
                {
                    Success = true,
                    Message = "Reporte generado exitosamente",
                    Data = reporte
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Error al generar reporte",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtener reporte detallado de ventas por período
        /// </summary>
        [HttpGet("reporte-detallado")]
        public async Task<IActionResult> GetReporteDetallado([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            try
            {
                var reporte = await _ventaService.ObtenerReporteDetalladoAsync(fechaInicio, fechaFin);

                return Ok(new
                {
                    Success = true,
                    Message = "Reporte detallado generado exitosamente",
                    Data = reporte
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Error al generar reporte detallado",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Descargar reporte en PDF
        /// </summary>
        [HttpGet("reporte-pdf")]
        public async Task<IActionResult> DownloadPDF([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            try
            {
                var pdfBytes = await _ventaService.GenerarReportePDFAsync(fechaInicio, fechaFin);

                return File(pdfBytes, "application/pdf", $"Reporte_Ventas_{fechaInicio:yyyyMMdd}_{fechaFin:yyyyMMdd}.pdf");
            }
            catch (NotImplementedException)
            {
                return StatusCode(501, new
                {
                    Success = false,
                    Message = "Funcionalidad de PDF no implementada aún"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Error al generar PDF",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Descargar reporte en Excel
        /// </summary>
        [HttpGet("reporte-excel")]
        public async Task<IActionResult> DownloadExcel([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            try
            {
                var excelBytes = await _ventaService.GenerarReporteExcelAsync(fechaInicio, fechaFin);

                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"Reporte_Ventas_{fechaInicio:yyyyMMdd}_{fechaFin:yyyyMMdd}.xlsx");
            }
            catch (NotImplementedException)
            {
                return StatusCode(501, new
                {
                    Success = false,
                    Message = "Funcionalidad de Excel no implementada aún"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Error al generar Excel",
                    Error = ex.Message
                });
            }
        }
    }
}

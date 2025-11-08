using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaVentas.Web.Models.ViewModels;
using SistemaVentas.Web.Services.Interfaces;

namespace SistemaVentas.Web.Controllers
{
    public class VentasController : Controller
    {
        private readonly IVentaApiService _ventaService;
        private readonly IProductoApiService _productoService;

        public VentasController(IVentaApiService ventaService, IProductoApiService productoService)
        {
            _ventaService = ventaService;
            _productoService = productoService;
        }

        private UsuarioSesion ObtenerUsuarioSesion()
        {
            var usuarioJson = HttpContext.Session.GetString("Usuario");
            if (string.IsNullOrEmpty(usuarioJson))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<UsuarioSesion>(usuarioJson);
        }

        public async Task<IActionResult> Index()
        {
            var usuario = ObtenerUsuarioSesion();
            if (usuario == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Solo administradores pueden ver el listado
            if (usuario.Rol != "Administrador")
            {
                return RedirectToAction("Create");
            }

            try
            {
                var ventas = await _ventaService.ObtenerVentasAsync(null, null);
                return View(ventas);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar ventas: " + ex.Message;
                return View(new List<VentaViewModel>());
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            var usuario = ObtenerUsuarioSesion();
            if (usuario == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var model = new CrearVentaViewModel
            {
                Vendedor = usuario.NombreCompleto,
                IdUsuario = usuario.IdUsuario
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearVentaViewModel model)
        {
            var usuario = ObtenerUsuarioSesion();
            if (usuario == null)
            {
                return Json(new { success = false, message = "Sesión expirada" });
            }

            try
            {
                model.IdUsuario = usuario.IdUsuario;
                var idVenta = await _ventaService.CrearVentaAsync(model);

                return Json(new
                {
                    success = true,
                    message = "Venta registrada exitosamente",
                    idVenta = idVenta
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> Reportes()
        {
            var usuario = ObtenerUsuarioSesion();
            if (usuario == null || usuario.Rol != "Administrador")
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerReporte(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                var ventas = await _ventaService.ObtenerVentasAsync(fechaInicio, fechaFin);
                return Json(new { success = true, data = ventas });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DescargarPDF(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                var pdfBytes = await _ventaService.DescargarReportePDFAsync(fechaInicio, fechaFin);
                return File(pdfBytes, "application/pdf", $"Reporte_Ventas_{fechaInicio:yyyyMMdd}_{fechaFin:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al generar PDF: " + ex.Message;
                return RedirectToAction("Reportes");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DescargarExcel(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                var excelBytes = await _ventaService.DescargarReporteExcelAsync(fechaInicio, fechaFin);
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"Reporte_Ventas_{fechaInicio:yyyyMMdd}_{fechaFin:yyyyMMdd}.xlsx");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al generar Excel: " + ex.Message;
                return RedirectToAction("Reportes");
            }
        }
    }
}
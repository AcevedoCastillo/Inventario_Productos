using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaVentas.Web.Models.ViewModels;
using SistemaVentas.Web.Services.Interfaces;

namespace SistemaVentas.Web.Controllers
{
    public class ProductosController : Controller
    {
        private readonly IProductoApiService _productoService;

        public ProductosController(IProductoApiService productoService)
        {
            _productoService = productoService;
        }

        // Middleware para verificar sesión y rol
        private bool VerificarSesionAdmin()
        {
            var usuarioJson = HttpContext.Session.GetString("Usuario");
            if (string.IsNullOrEmpty(usuarioJson))
            {
                return false;
            }

            var usuario = JsonConvert.DeserializeObject<UsuarioSesion>(usuarioJson);
            return usuario.Rol == "Administrador";
        }

        public async Task<IActionResult> Index()
        {
            if (!VerificarSesionAdmin())
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var productos = await _productoService.ObtenerTodosAsync();
                return View(productos);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar productos: " + ex.Message;
                return View(new List<ProductoViewModel>());
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!VerificarSesionAdmin())
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductoViewModel model)
        {
            if (!VerificarSesionAdmin())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _productoService.CrearAsync(model);
                TempData["SuccessMessage"] = "Producto creado exitosamente";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al crear producto: " + ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!VerificarSesionAdmin())
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var producto = await _productoService.ObtenerPorIdAsync(id);
                return View(producto);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar producto: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ProductoViewModel model)
        {
            if (!VerificarSesionAdmin())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _productoService.ActualizarAsync(id, model);
                TempData["SuccessMessage"] = "Producto actualizado exitosamente";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al actualizar producto: " + ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (!VerificarSesionAdmin())
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var producto = await _productoService.ObtenerPorIdAsync(id);
                return View(producto);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar producto: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int IdPro)
        {
            if (!VerificarSesionAdmin())
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                await _productoService.EliminarAsync(IdPro);
                TempData["SuccessMessage"] = "Producto eliminado exitosamente";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al eliminar producto: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // API para búsqueda en ventas
        [HttpGet]
        public async Task<IActionResult> BuscarPorCodigo(string codigo)
        {
            try
            {
                var producto = await _productoService.ObtenerPorCodigoAsync(codigo);
                return Json(new { success = true, data = producto });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaVentas.Web.Models.ViewModels;

namespace SistemaVentas.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Verificar sesión
            var usuarioJson = HttpContext.Session.GetString("Usuario");
            if (string.IsNullOrEmpty(usuarioJson))
            {
                return RedirectToAction("Login", "Auth");
            }

            var usuario = JsonConvert.DeserializeObject<UsuarioSesion>(usuarioJson);

            // Verificar rol de administrador
            if (usuario.Rol != "Administrador")
            {
                return RedirectToAction("Create", "Ventas");
            }

            return View();
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SistemaVentas.Web.Models.ViewModels;
using SistemaVentas.Web.Services.Interfaces;

namespace SistemaVentas.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IApiService _apiService;

        public AuthController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Si ya está logueado, redirigir
            var usuario = HttpContext.Session.GetString("Usuario");
            if (!string.IsNullOrEmpty(usuario))
            {
                var usuarioObj = JsonConvert.DeserializeObject<UsuarioSesion>(usuario);
                return usuarioObj.Rol == "Administrador"
                    ? RedirectToAction("Index", "Home")
                    : RedirectToAction("Create", "Ventas");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var loginData = new
                {
                    nombreUsuario = model.NombreUsuario,
                    contrasena = model.Contrasena
                };

                var response = await _apiService.PostAsync<JObject>("/auth/login", loginData);

                if (response["success"].ToObject<bool>())
                {
                    var usuario = response["data"].ToObject<UsuarioSesion>();

                    // Guardar en sesión
                    HttpContext.Session.SetString("Usuario", JsonConvert.SerializeObject(usuario));

                    TempData["SuccessMessage"] = "Bienvenido " + usuario.NombreCompleto;

                    // Redirigir según rol
                    if (usuario.Rol == "Administrador")
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return RedirectToAction("Create", "Ventas");
                    }
                }

                ModelState.AddModelError("", "Usuario o contraseña incorrectos");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al iniciar sesión: " + ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
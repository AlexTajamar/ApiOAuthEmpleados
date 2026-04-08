using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MvcOAuthApiEmpleados.Models;
using MvcOAuthApiEmpleados.Services;
using System.Security.Claims;

namespace MvcOAuthApiEmpleados.Controllers
{
    public class ManagedController : Controller
    {
        private ServiceEmpleados service;

        public ManagedController(ServiceEmpleados service)
        {
            this.service = service;
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LogicModel model)
        {
            string token = await this.service.LogInAsync(model.UserName, model.Password);
            if (token == null)
            {
                ViewData["mensajelogin"] = "El usuario o el password es incorrecto.";
                return View();
            }
            else
            {
                HttpContext.Session.SetString("TOKEN", token);
                ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);

                Claim claimName = new Claim(ClaimTypes.Name, model.UserName);
                identity.AddClaim(claimName);

                Claim claimId = new Claim(ClaimTypes.NameIdentifier, model.Password);
                identity.AddClaim(claimId);

                identity.AddClaim(new Claim("TOKEN", token));

                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                // Se da al usuario de alta durante 10 minutos
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(10)
                });

                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}

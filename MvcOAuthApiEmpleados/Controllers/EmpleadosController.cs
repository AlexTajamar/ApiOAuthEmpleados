using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcOAuthApiEmpleados.Models;
using MvcOAuthApiEmpleados.Services;
using MvcOAuthEmpleados.Filters;
using System.Security.Claims;

namespace MvcOAuthApiEmpleados.Controllers
{
    public class EmpleadosController : Controller
    {
        private ServiceEmpleados service;

        public EmpleadosController(ServiceEmpleados service)
        {
            this.service = service;
        }

        [AuthorizeEmpleados]
        public async Task<IActionResult> Index()
        {
            List<Empleado> empleados = await this.service.GetEmpleadosAsync();
            return View(empleados);
        }
        [AuthorizeEmpleados]
        //AL PASAR POR AQUI EN EL CLAIM TENDRIAMSO EL TOKEN
        public async Task<IActionResult> Details(int id)
        {
           
               Empleado empleado = await this.service.FindEmpleadoAsync(id);
              return View(empleado);
            
        }

        public async Task<ActionResult> EmpleadosOficios()
        {
            List<string> oficios = await this.service.GetOficiosAsync();
            ViewData["OFICIOS"] = oficios;
            return View();

        }
        [HttpPost]
        public async Task<ActionResult> EmpleadosOficios(int? incremento,List<string> oficio,string accion)
        {
            List<string> oficios = await this.service.GetOficiosAsync();
            ViewData["OFICIOS"] = oficios;
            if (accion.ToLower() == "update")
            {
                await this.service.UpdateEmpleadosAsync(incremento.Value, oficio);

            }
            List<Empleado> empleados = await this.service.GetEmpleadosOficiosAsync(oficio);
            return View(empleados);
        }


        [Authorize(Roles ="PRESIDENTE")]
        [AuthorizeEmpleados]
        //AL PASAR POR AHI EN EL CLAIM TENDRIAMSO EL TOKEN
        public async Task<IActionResult> PerfilEmpleado()
        {
            var data = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (data == null)
            {
                return Unauthorized();
            }
            int idEmpleado = int.Parse(data);
            Empleado empleado = await this.service.FindEmpleadoAsync(idEmpleado);
            return View(empleado);
        }

        [AuthorizeEmpleados]
        public async Task<ActionResult> Perfil()
        {
            Empleado empleado = await this.service.PerfilEmpleadoAsync();
            return View(empleado);
        }

        [AuthorizeEmpleados]
        public async Task<ActionResult> Compis()
        {
           List<Empleado> empleados = await this.service.GetCompisAsync();
            return View(empleados);
        }


    }
}

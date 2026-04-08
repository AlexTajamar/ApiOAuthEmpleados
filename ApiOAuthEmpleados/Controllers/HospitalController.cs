using ApiOAuthEmpleados.Models;
using ApiOAuthEmpleados.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using ApiOAuthEmpleados.Helpers;

namespace ApiOAuthEmpleados.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HospitalController : ControllerBase
    {
        public RepositoryHospital repo;
        private HelperActionOAuthService helper;

        public HospitalController(RepositoryHospital repository, HelperActionOAuthService helper)
        {
            repo = repository;
            this.helper = helper;
        }

        [HttpGet]
    public async Task<ActionResult<List<Empleado>>> GetEmpleados()
        {
            return await this.repo.GetEmpleadosAsync();
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Empleado>> FindEmpleado(int id)
        {
            return await this.repo.FindEmpleadoAsync(id);

        }


        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<Empleado>> Perfil()
        {
            Claim claim = HttpContext.User.FindFirst(z=>z.Type == "EmpleadoData");
            string json = claim.Value; //INFO DEL USUARIO
            string jsonDescifrado = await HelperCifrado.DecryptStringAsync(json, this.helper.SecretKey);
            Empleado empleado = JsonConvert.DeserializeObject<Empleado>(jsonDescifrado);
            return await this.repo.FindEmpleadoAsync(empleado.IdEmpleado);

        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<Empleado>>> Compis()
        {
            Claim claim = HttpContext.User.FindFirst(z => z.Type == "EmpleadoData");
            string json = claim.Value; //INFO DEL USUARIO
            string jsonDescifrado = await HelperCifrado.DecryptStringAsync(json, this.helper.SecretKey);
            Empleado empleado = JsonConvert.DeserializeObject<Empleado>(jsonDescifrado);
            return await this.repo.GetCompisAsync(empleado.IdDepartamento);

        }
    }
}
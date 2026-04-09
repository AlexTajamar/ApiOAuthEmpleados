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
        private HelperEmpleadoToken helperEmpleadoToken;

        public HospitalController(RepositoryHospital repository, HelperActionOAuthService helper, HelperEmpleadoToken helperEmpleadoToken)
        {
            repo = repository;
            this.helper = helper;
            this.helperEmpleadoToken = helperEmpleadoToken;
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
            EmpleadoModel empleadoModel = await this.helperEmpleadoToken.GetEmpleado();
            return await this.repo.FindEmpleadoAsync(empleadoModel.IdEmpleado);
        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<Empleado>>> Compis()
        {
            EmpleadoModel empleadoModel = await this.helperEmpleadoToken.GetEmpleado();
            return await this.repo.GetCompisAsync(empleadoModel.IdDepartamento);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<string>>> Oficios()
        {
            return await this.repo.GetOficiosAsync();
        }
        //Este recibirá por url una lista de oficios y nos devolverá los empleados que tengan esos oficios
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<Empleado>>> EmpleadosByOficios([FromQuery] List<string> oficios)
        {
            return await this.repo.GetEmpleadosByOficiosAsync(oficios);
        }

        [HttpPut]
        [Route("[action]/{incremento}")]
        public async Task<ActionResult> IncrementarSalarios(int incremento, [FromQuery] List<string> oficios)
        {
            await this.repo.IncrementarSalariosAsync(incremento, oficios);
            return Ok();
        } 
    }
}
           

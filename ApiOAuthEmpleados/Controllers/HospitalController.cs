using ApiOAuthEmpleados.Models;
using ApiOAuthEmpleados.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiOAuthEmpleados.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HospitalController : ControllerBase
    {
        public RepositoryHospital repo;
        public HospitalController(RepositoryHospital repository)
        {
            repo = repository;
        }

        [HttpGet]
    public async Task<ActionResult<List<Empleado>>> GetEmpleados()
        {
            return await this.repo.GetEmpleadosAsync();
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Empleado>> FindEmpleado(int id)
        {
            return await this.repo.FindEmpleadoAsync(id);

        }
    }
}
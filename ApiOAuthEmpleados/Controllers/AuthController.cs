using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ApiOAuthEmpleados.Models;
using Microsoft.IdentityModel.Tokens;
using ApiOAuthEmpleados.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Security.Claims;

namespace ApiOAuthEmpleados.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private Repositories.RepositoryHospital repo;
        private HelperActionOAuthService helper;
        
        // Inject both the repository and the helper
        public AuthController(Repositories.RepositoryHospital repo, HelperActionOAuthService helper)
        {
            this.repo = repo;
            this.helper = helper;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult>Login(LogicModel model)
        {
            Empleado empleado = await this.repo.LogInEmpleado(model.UserName, int.Parse(model.Password));

            if (empleado == null)
            {
                return Unauthorized();
            }
            else
            {
                SigningCredentials credentials = new SigningCredentials(this.helper.GetKeyToken(), SecurityAlgorithms.HmacSha256);

                //CREAMOS NUESTRO MODELO PARA ALMACENARLO EN EL TOKEN
                EmpleadoModel empleadoModel = new EmpleadoModel
                {
                    IdEmpleado = empleado.IdEmpleado,
                    Apellido = empleado.Apellido,
                    Oficio = empleado.Oficio,
                    Salario = empleado.Salario,
                    IdDepartamento = empleado.IdDepartamento
                };



                string jsonEmpleado = JsonConvert.SerializeObject(empleadoModel);
                
                string jsonCifrado = await HelperCifrado.EncryptStringAsync(jsonEmpleado, this.helper.SecretKey);

                Claim[] info = new[]
                {
                    new Claim("EmpleadoData", jsonCifrado),
                    new Claim(ClaimTypes.Role, empleado.Oficio)
                };
                JwtSecurityToken token = new JwtSecurityToken(
                    claims: info,
                    issuer: this.helper.Issuer,
                    audience: this.helper.Audience,
                    // CAMBIO AQUI: Utiliza DateTime.UtcNow para el token en vez de Now
                    expires: DateTime.UtcNow.AddMinutes(15),
                    signingCredentials: credentials,
                    notBefore: DateTime.UtcNow
                    );

                return Ok(new
                {
                    Response = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
        }
    }
}





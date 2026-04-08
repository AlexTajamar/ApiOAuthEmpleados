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
                
                string jsonEmpleado = JsonConvert.SerializeObject(empleado);
                
                string jsonCifrado = await HelperCifrado.EncryptStringAsync(jsonEmpleado, this.helper.SecretKey);

                Claim[] info = new[]
                {
                    new Claim("EmpleadoData", jsonCifrado)
                };
                JwtSecurityToken token = new JwtSecurityToken(
                    claims: info,
                    issuer: this.helper.Issuer,
                    audience: this.helper.Audience,
                    expires: DateTime.Now.AddMinutes(15),
                    signingCredentials: credentials,
                    notBefore: DateTime.Now
                    );

                return Ok(new
                {
                    Response = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
        }
    }
}




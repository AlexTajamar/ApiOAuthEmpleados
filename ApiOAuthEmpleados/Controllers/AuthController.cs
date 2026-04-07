using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ApiOAuthEmpleados.Models;
using Microsoft.IdentityModel.Tokens;
using ApiOAuthEmpleados.Helpers;
using System.IdentityModel.Tokens.Jwt;

namespace ApiOAuthEmpleados.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private Repositories.RepositoryHospital repo;
        private HelperActionOAuthService helper;
        
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
                JwtSecurityToken token = new JwtSecurityToken(
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
        }}
    }


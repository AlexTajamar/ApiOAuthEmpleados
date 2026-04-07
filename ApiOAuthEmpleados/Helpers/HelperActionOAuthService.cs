using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ApiOAuthEmpleados.Helpers
{
    public class HelperActionOAuthService
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecretKey { get; set; }

        public HelperActionOAuthService(IConfiguration configuration)
        {
            Issuer = configuration.GetValue<string>("ApiOAuthToken:Issuer");
            Audience = configuration.GetValue<string>("ApiOAuthToken:Audience");
            SecretKey = configuration.GetValue<string>("ApiOAuthToken:SecretKey");
        }


        public SymmetricSecurityKey GetKeyToken()
        {
            byte[] data = Encoding.UTF8.GetBytes(this.SecretKey);
            return new SymmetricSecurityKey(data);
        }

        public Action<JwtBearerOptions> GetJwtBeaberOptions()
        {
            Action<JwtBearerOptions> options = new Action<JwtBearerOptions>(opt =>
            {
               opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = this.Issuer,
                    ValidAudience = this.Audience,
                    IssuerSigningKey = this.GetKeyToken(),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };
            });
            return options; 
        }

        public Action<AuthenticationOptions> GetAuthSchema()
        {
            Action<AuthenticationOptions> options = new Action<AuthenticationOptions>(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
            });
            return options;
        }
    }
}

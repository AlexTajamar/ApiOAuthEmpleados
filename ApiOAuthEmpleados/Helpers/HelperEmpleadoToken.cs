using ApiOAuthEmpleados.Models;
using Newtonsoft.Json;

namespace ApiOAuthEmpleados.Helpers
{
    public class HelperEmpleadoToken
    {
        private IHttpContextAccessor httpContextAccessor;
        private HelperActionOAuthService helperActionOAuth; // Añadimos esto para obtener el SecretKey

        public HelperEmpleadoToken(IHttpContextAccessor httpContextAccessor, HelperActionOAuthService helperActionOAuth)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.helperActionOAuth = helperActionOAuth;
        }

        // Cambiamos el método a asíncrono porque la desencriptación es asíncrona
        public async Task<EmpleadoModel> GetEmpleado() 
        {
            string jsonCifrado = httpContextAccessor.HttpContext?.User?.FindFirst("EmpleadoData")?.Value;
            
            if (string.IsNullOrEmpty(jsonCifrado))
            {
                return null;
            }

            // Aquí desencriptamos la información ANTES de deserializarla
            string jsonEmpleado = await HelperCifrado.DecryptStringAsync(jsonCifrado, this.helperActionOAuth.SecretKey);

            return JsonConvert.DeserializeObject<EmpleadoModel>(jsonEmpleado);
        }
    }
}

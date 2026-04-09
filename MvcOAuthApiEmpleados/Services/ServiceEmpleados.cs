using MvcOAuthApiEmpleados.Models;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace MvcOAuthApiEmpleados.Services
{
    public class ServiceEmpleados
    {
        private string UrlApi;
        private MediaTypeWithQualityHeaderValue header;
        private IHttpContextAccessor accessor;

        public ServiceEmpleados(IConfiguration configuration, IHttpContextAccessor accessor)
        {
            UrlApi = configuration.GetConnectionString("ApiEmpleados");
            header = new MediaTypeWithQualityHeaderValue("application/json");
            this.accessor = accessor;
        }

        public async Task<string> LogInAsync(string user, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/Auth/Login";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(header);
                LogicModel model = new LogicModel()
                {
                    UserName = user,
                    Password = password
                };
                string json = System.Text.Json.JsonSerializer.Serialize(model);
                StringContent content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    JObject jsonData = JObject.Parse(data);
                    string token = jsonData.GetValue("response").ToString();
                    return token;
                }
                else
                {
                    return null;
                }



            }
        }


        public async Task<T> CallApiAsync<T>(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(header);
                HttpResponseMessage response = await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }


        public async Task<T> CallApiAsync<T>(string token, string request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(header);

                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                HttpResponseMessage response = await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        public async Task<List<Empleado>> GetEmpleadosAsync()
        {
            string request = "api/Hospital";
            List<Empleado> empleados = await CallApiAsync<List<Empleado>>(request);
            return empleados;
        }

        public async Task<Empleado> FindEmpleadoAsync(int id)
        {
            string token = accessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "TOKEN").Value;
            string request = "api/Hospital/" + id;
            return await CallApiAsync<Empleado>(token, request);
        }

        public async Task<List<Empleado>> GetCompisAsync()
        {
            string token = accessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "TOKEN").Value;
            string request = "api/Hospital/Compis";
            List<Empleado> empleados = await CallApiAsync<List<Empleado>>(token, request);
            return empleados;
        }

        public async Task<Empleado> PerfilEmpleadoAsync()
        {
            string token = accessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "TOKEN").Value;
            string request = "api/Hospital/Perfil";
            Empleado empleado = await CallApiAsync<Empleado>(token, request);
            return empleado;
        }

        //necesitamos generar el siguietne request oficio=ANALISTA oficio=DIRECTOR a partir de un List<string>
        //vamos a crear un metodo wue devuelva este string a partir de un List<string>

        public string GetOficiosRequest(List<string> oficios)
        {
            string result = "";
            for (int i = 0; i < oficios.Count; i++)
            {
                // Cambiado "oficio=" a "oficios=" para coincidir con el [FromQuery] de la API
                result += "oficios=" + oficios[i];
                if (i < oficios.Count - 1)
                {
                    result += "&";
                }
            }
            return result;
        }

        public async Task<List<Empleado>> GetEmpleadosOficiosAsync(List<string> oficios)
        {
            // Faltaba el símbolo '?' para iniciar el QueryString
            string request = "api/Hospital/EmpleadosByOficios?" + GetOficiosRequest(oficios);
            List<Empleado> empleados = await CallApiAsync<List<Empleado>>(request);
            return empleados;
        }

        public async Task UpdateEmpleadosAsync(int incremento, List<string> oficios)
        {
            // El 'incremento' es un path parameter, no requiere llave valor "incremento=x"
            // y agregamos '?' para arrancar el QueryString de oficios
            string request = "api/Hospital/IncrementarSalarios/" + incremento + "?" + GetOficiosRequest(oficios);
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(header);
                HttpResponseMessage response = await client.PutAsync(request, null);

            }
        }

        public async Task<List<string>> GetOficiosAsync()
        {
            string request = "api/Hospital/Oficios";
            List<string> oficios = await CallApiAsync<List<string>>(request);
            return oficios;
        }
    }
}

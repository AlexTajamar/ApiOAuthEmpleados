using ClienteApiOAuthEmpleados;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

Console.WriteLine("Introduce el user: ");
string us = Console.ReadLine();
Console.WriteLine("Introduce el password: ");
string pass = Console.ReadLine();
string resultado = await GetTokenAsync(us, pass);

Console.WriteLine("Resultado: " + resultado);
Console.WriteLine("---------------------------------------------");
Console.WriteLine("Introduciendo idEmpleado");
string idEmpleado = Console.ReadLine();
string resultadoEmpleados = await FindEmpleadosAsync(resultado);
Console.WriteLine("Resultado Empleados: " + resultadoEmpleados);

// Para crear métodos en Program, deben ser static
static async Task<string> GetTokenAsync(string user, string password)
{
    string urlApi = "https://apioauthempleadosarg-fjdyd4byc9ctbsaa.germanywestcentral-01.azurewebsites.net/";
    LogicModel model = new LogicModel()
    {
        UserName = user,
        Password = password
    };

    using (HttpClient client = new HttpClient())
    {
        string request = "api/auth/login";
        client.BaseAddress = new Uri(urlApi);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        string json = JsonConvert.SerializeObject(model);

        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await client.PostAsync(request, content);

        if (response.IsSuccessStatusCode)
        {
            string data = await response.Content.ReadAsStringAsync();
            JObject objeto = JObject.Parse(data);
            string token = objeto.GetValue("response").ToString();

            return token;
        }
        else
        {
            return "Incorrect petition: " + response.StatusCode;
        }
    }
}

static async Task<string> FindEmpleadosAsync(string token)
{
    string urlApi = "https://apioauthempleadosarg-fjdyd4byc9ctbsaa.germanywestcentral-01.azurewebsites.net/";
    string request = "api/hospital";

    using (HttpClient client = new HttpClient())
    {
        client.BaseAddress = new Uri(urlApi);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        HttpResponseMessage response = await client.GetAsync(request);
        if (response.IsSuccessStatusCode)
        {
            string data = await response.Content.ReadAsStringAsync();
            return data;
        }
        else
        {
            return "Incorrect petition: " + response.StatusCode;
        }
    }
}
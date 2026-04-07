// See https://aka.ms/new-console-template for more information
using ClienteApiOAuthEmpleados;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using static System.Net.WebRequestMethods;

Console.WriteLine("Hello, World!");

static async Task<string>GetTokenAsync(string user,string pass)
{
    string urlApi = "https://apioauthempleadosarg-fjdyd4byc9ctbsaa.germanywestcentral-01.azurewebsites.net";
    LogicModel model = new LogicModel();

    model.UserName = user;
    model.Password = pass;

    using (HttpClient client = new HttpClient())
    {
        string request = "api/Auth/Login";
        client.BaseAddress = new Uri(urlApi);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        string json = JsonConvert.SerializeObject(model);
        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await client.PostAsync(request, content);


    }
}
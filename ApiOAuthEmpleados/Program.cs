using ApiOAuthEmpleados.Data;
using ApiOAuthEmpleados.Repositories;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
string connectionString = builder.Configuration.GetConnectionString("SqlHospital");
builder.Services.AddDbContext<HospitalContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddTransient<RepositoryHospital>();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
   
}
app.MapOpenApi();
app.MapScalarApiReference();
app.MapGet("/",context => { context.Response.Redirect("/scalar");
return Task.CompletedTask; });

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

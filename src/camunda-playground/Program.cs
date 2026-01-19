using camunda_playground;
using camunda_playground.Services;
using System.Reflection;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//public class BaseWorker { }

builder.AddCamunda(
    assembly: Assembly.GetExecutingAssembly(),
    bootstrap: false
);
builder.Services.AddHealthChecks()
    .AddCheck<ZeebeHealthCheck>("zeebe");
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health/ready");


app.Run();

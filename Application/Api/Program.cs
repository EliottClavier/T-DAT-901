using Api.Config;
using Application.Config;
using Confluent.Kafka;
using Infrastructure.Config;
using Infrastructure.Kafka;
using Microsoft.Extensions.Hosting;
using System.Collections;
using System.Net;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.TraversePath().Load();

builder.Configuration.Sources.Clear();
builder.Configuration
    .AddEnvironmentVariables()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .Add(new CustomEnvironmentVariablesConfigurationSource());


builder.Services.AddLogging();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "front", policy =>
    {
        policy
        .AllowAnyHeader()
        .AllowAnyOrigin()
        .AllowAnyMethod();
    }
        );
}
    );

var app = builder.Build();

app.MapControllers();

app.MapGet("/", () => "Hello Crypto World!");

app.UseCors("front");

app.Run();



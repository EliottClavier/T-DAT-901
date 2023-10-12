using Api.Config;
using Application.Config;
using Confluent.Kafka;
using Infrastructure.Config;
using Infrastructure.Kafka;
using System.Collections;
using System.Net;
using System.Reflection;

DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.Sources.Clear();
builder.Configuration
    .AddEnvironmentVariables()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .Add(new CustomEnvironmentVariablesConfigurationSource())
    .AddCommandLine(args);


builder.Services.AddLogging();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);


var app = builder.Build();


app.MapGet("/", () => "Hello Crypto World!");

app.Run();



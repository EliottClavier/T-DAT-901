using Application.Config;
using Infrastructure.Config;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddInfrastructure(builder.Configuration);  
builder.Services.AddApplication(builder.Configuration);


var app = builder.Build();


app.MapGet("/", () => "Hello Crypto World!");

app.Run();

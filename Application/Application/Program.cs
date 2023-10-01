using Core;
using Domain;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSingleton<ICryptoScraperService, BitCoinCryptoScraperService>();
builder.Services.AddHostedService<CryptoPriceService>();

builder.Services.AddSingleton<KafkaProducerService>(new KafkaProducerService("localhost:9092")); // Remplacez par l'adresse de votre serveur Kafka

//builder.Services.AddHostedService(provider => (IHostedService)provider.GetRequiredService<ICryptoPriceService>());


var app = builder.Build();



//app.MapGet("/", () => "Hello World!");

app.Run();

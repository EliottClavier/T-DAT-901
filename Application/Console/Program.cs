using System;
using System.Security.Cryptography;
using Application.Config;
using Infrastructure.Config;
using Domain.Ports;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Core;
using Infrastructure.Kafka;
using Infrastructure.Scraper;

var serviceCollection = new ServiceCollection();
IConfiguration configuration = new ConfigurationBuilder().Build();

// Configurez l'injection de dépendances
serviceCollection.AddApplication(configuration);
serviceCollection.AddInfrastructure(configuration);
serviceCollection.AddSingleton<CryptoHostedService>();
 serviceCollection.AddLogging(configure => configure.AddConsole().SetMinimumLevel(LogLevel.Information));
serviceCollection.AddSingleton(new KafkaProducerService("localhost:9092"));
//serviceCollection.AddSingleton<ICryptoScraperService, CoinMarketCapCryptoScraperService>();

// Construisez le fournisseur de services
var serviceProvider = serviceCollection.BuildServiceProvider();



// Récupérez le service et utilisez-le
var cryptoService = serviceProvider.GetService<CryptoHostedService>();
var logger = serviceProvider.GetService<ILogger<Program>>();
logger.LogInformation("Starting the application");
cryptoService?.StartAsync(CancellationToken.None);



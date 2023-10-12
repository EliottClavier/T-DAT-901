
using Domain;
using Domain.Domain;
using Domain.Ports;
using Infrastructure.Kafka;
using Infrastructure.Scraper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;

namespace Infrastructure.Config
{
    public static class DependencyInjection
    {
            public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
            {
                services.AddSingleton(new KafkaProducerService("localhost:9092"));

            services.AddSingleton<Func<ExchangeScrappingInfo, ICryptoScraperService>>(provider =>
            {
                var logger = provider.GetService<ILogger<CryptoScrapperFactory>>();

                var factory = new CryptoScrapperFactory(logger);

                return info => factory.Create(info);
            });


            return services;
            }

    }
}

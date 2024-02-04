
using Domain;
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

            services.Configure<KafkaSettings>(configuration.GetSection("KAFKA"));

            services.AddSingleton<KafkaProducerService>();

            var enableScraper = configuration.GetValue<bool>("ENABLE_SCRAPER");
            if(enableScraper)
            {
                services.AddSingleton<Func<ExchangeScrappingInfo, ICryptoScraperService>>(provider =>
                {
                    var logger = provider.GetService<ILogger<CryptoScrapperFactory>>();

                    using (var factory = new CryptoScrapperFactory(logger))
                    {
                        return info => factory.Create(info);
                    }
                });
            }

            return services;
        }

    }
}

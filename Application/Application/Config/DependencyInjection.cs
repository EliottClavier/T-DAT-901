using Core;
using Domain;
using Domain.Ports;
using Infrastructure.Kafka;
using Infrastructure.Socket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;


namespace Application.Config
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddLogging();

            var enableScraper = configuration.GetValue<bool>("ENABLE_SCRAPER");
            if (enableScraper)
            {
                services.AddHostedService<CryptoHostedService>();
                services.AddSingleton<IExchangeScrappingInfoProvider, AppConfigExchangeScrappingInfoProvider>();

            }

            var enableStreamer = configuration.GetValue<bool>("ENABLE_STREAMER");
            if (enableStreamer)
            {
                services.AddHostedService<WebSocketStreamer>();
                services.Configure<WebSocketConfig>(configuration.GetSection("WebSocketConfig"));
                services.AddSingleton(provider => provider.GetRequiredService<IOptions<WebSocketConfig>>().Value);

            }


            // Enregistrement du HistoricalTradeService
            services.AddSingleton<HistoricalTradeService>();

            return services;
        }
    }
}


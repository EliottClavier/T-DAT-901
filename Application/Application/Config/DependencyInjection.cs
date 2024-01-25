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
            
            services.AddHostedService<CryptoHostedService>();
            services.AddHostedService<WebSocketStreamer>();
            services.AddSingleton<IExchangeScrappingInfoProvider, AppConfigExchangeScrappingInfoProvider>();

            // Ajoute la configuration de la classe WebSocketConfig
            services.Configure<WebSocketConfig>(configuration.GetSection("WebSocketConfig"));
            // Ajoute la classe WebSocketConfig comme service
            services.AddSingleton(provider => provider.GetRequiredService<IOptions<WebSocketConfig>>().Value);

            //// Configuration Kafka
            //services.Configure<KafkaSettings>(configuration.GetSection("KAFKA"));

            //// Enregistrement du KafkaProducerService
            //services.AddSingleton<KafkaProducerService>();

            // Enregistrement du HistoricalTradeService
            services.AddSingleton<HistoricalTradeService>();

            return services;
        }
        //public static IServiceCollection AddApplicationCMD(this IServiceCollection services,
        //    IConfiguration configuration)
        //{

        //    services.AddSingleton<CryptoHostedService>();

        //    return services;
        //}
    }
}


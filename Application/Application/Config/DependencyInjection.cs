using Core;
using Domain.Ports;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Application.Config
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services,
            IConfiguration configuration)
        {
            
            services.AddHostedService<CryptoHostedService>();
            services.AddSingleton<IExchangeScrappingInfoProvider, AppConfigExchangeScrappingInfoProvider>();

            return services;
        }
        public static IServiceCollection AddApplicationCMD(this IServiceCollection services,
            IConfiguration configuration)
        {

            services.AddSingleton<CryptoHostedService>();

            return services;
        }
    }
}


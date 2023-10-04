using Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Config
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services,
            IConfiguration configuration)
        {
            
            services.AddHostedService<CryptoHostedService>();

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


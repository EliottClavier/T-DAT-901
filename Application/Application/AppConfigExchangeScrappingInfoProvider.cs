using Domain;
using Domain.Ports;
using Microsoft.Extensions.Configuration;

namespace Application
{
    public class AppConfigExchangeScrappingInfoProvider : IExchangeScrappingInfoProvider
    {
        private readonly IConfiguration _configuration;

        public AppConfigExchangeScrappingInfoProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<ExchangeScrappingInfo>? GetExchangeScrappingInfo()
        {
            return _configuration.GetSection("ExchangeScrappingInfo").Get<IEnumerable<ExchangeScrappingInfo>>();
        }
    }

}
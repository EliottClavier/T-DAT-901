using Domain;
using Domain.Domain;
using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



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
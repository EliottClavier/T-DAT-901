
using Domain.Ports;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Infrastructure.WebDriver;
using OpenQA.Selenium.Remote;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Scraper
{
    public class CryptoScrapperFactory
    {
        private readonly ILogger _logger;

        public CryptoScrapperFactory( ILogger logger) {
        
        _logger = logger;

        }

        public ICryptoScraperService Create(ExchangeScrappingInfo info )
        {
            if (info != null)
            {
                var driverFactory = new ChromWebDriverFactory(_logger);

                var scraperService = new CryptoScraperService(info, driverFactory.CreateDriver());
               
                return scraperService;                          
     
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }
    }


}

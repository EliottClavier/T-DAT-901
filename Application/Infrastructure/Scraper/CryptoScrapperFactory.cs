
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
    public class CryptoScrapperFactory : IDisposable
    {
        private readonly ILogger _logger;
        private readonly ChromWebDriverFactory _driver;
        private ChromWebDriverFactory _driverFactory;
        private CryptoScraperService _scraperService;

        public CryptoScrapperFactory(ILogger logger)
        {

            _logger = logger;

        }

        public ICryptoScraperService Create(ExchangeScrappingInfo info)
        {
            if (info != null)
            {
                _driverFactory = new ChromWebDriverFactory(_logger);
                _scraperService = new CryptoScraperService(info, _driverFactory.CreateDriver());

                return _scraperService;

            }
            else
            {
                throw new KeyNotFoundException();
            }
        }

        public void Dispose()
        {
            _scraperService?.Dispose();
            _driverFactory?.Dispose();
        }
    }


}

using Domain.Domain;
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

namespace Infrastructure.Scraper
{
    public class CryptoScrapperFactory
    {
       

        public CryptoScrapperFactory( ) {
        
     

        }

        public ICryptoScraperService Create(ExchangeScrappingInfo info )
        {
            if (info != null)
            {
                var driverFactory = new ChromWebDriverFactory();
                return new CryptoScraperService(info, driverFactory.CreateDriver());
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }
    }


}

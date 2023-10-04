using Domain;
using Domain.Ports;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.DevTools;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;

namespace Infrastructure.Scraper
{
    public class BinanceScraperService : ICryptoScraperService
    {
        private readonly string _symbol;
        private readonly string _exchange;
        private readonly string _pairSymbol;
        private readonly string _url;
        private readonly ILogger<BinanceScraperService> _logger;
        private readonly ChromeDriver _driver;

        public BinanceScraperService(string symbol, string pairSymbol, string exchange, string url, ILogger<BinanceScraperService> logger)
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--disable-extensions");


            _driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), options, TimeSpan.FromMinutes(3));
            _symbol = symbol;
            _pairSymbol = pairSymbol;
            _exchange = exchange;
            _url = url;
            _logger = logger;


            _driver.Manage().Timeouts().PageLoad.Add(System.TimeSpan.FromSeconds(30));

        }

        public void Dispose()
        {

        }

        private void NavigateToUrl()
        {
            if (_driver.Url != _url)
            {
                _driver.Navigate().GoToUrl(_url);
            }

        }


        public CryptoData? GetCryptoInfoAsync()
        {
            NavigateToUrl();
            var price = ExtractData("//*[@id=\"tabContainer\"]/div[2]/div[3]/div/div/div[2]/div[1]/div/div[1]/div");
            var volume = ExtractData("//*[@id=\"tabContainer\"]/div[2]/div[3]/div/div/div[2]/div[1]/div/div[3]");


            return new CryptoData(_symbol, _pairSymbol, _symbol, _exchange, price, volume, "0", "0");
        }

        private string ExtractData(string xpath)
        {
            try {               
          
            var wait = new WebDriverWait(_driver, _driver.Manage().Timeouts().AsynchronousJavaScript);
            var element = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.XPath(xpath)));
          
            return (string)((IJavaScriptExecutor)_driver).ExecuteScript("return arguments[0].innerText;", element);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting data");
                return "0";
            }
        }
    }
}
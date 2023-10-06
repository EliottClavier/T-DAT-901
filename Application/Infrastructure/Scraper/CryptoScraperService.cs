using Domain;
using Domain.Ports;
using Microsoft.Extensions.Logging;

using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using Domain.Domain;
using System.Text.RegularExpressions;

namespace Infrastructure.Scraper
{
    public class CryptoScraperService : ICryptoScraperService
    {
        private readonly ChromeDriver _driver;
        private readonly ExchangeScrappingInfo _info;

        public CryptoScraperService(ExchangeScrappingInfo info)
        {
            var options = new ChromeOptions();
            //options.AddArgument("--headless");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--lang=en-US");

            _driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), options, TimeSpan.FromMinutes(3));     
            _driver.Manage().Timeouts().PageLoad.Add(System.TimeSpan.FromSeconds(30));

            _info = info;
        }
      
        private void NavigateToUrl()
        {
            if (_driver.Url != _info.Url)
            {
                _driver.Navigate().GoToUrl(_info.Url);
            }

        }


        public CryptoData? GetCryptoInfoAsync()
        {
            NavigateToUrl(); 
            var price = !string.IsNullOrEmpty(_info.PriceXPath) ? ExtractData(_info.PriceXPath) : "";
            var volume =!string.IsNullOrEmpty(_info.Volume24HXPath) ? ExtractData(_info.Volume24HXPath) : "";
            var supply = !string.IsNullOrEmpty(_info.CirculatingSupplyXPath) ? ExtractData(_info.CirculatingSupplyXPath) : "";

            return new CryptoData(_info.Currency, _info.CurrencyPair, _info.CurrencySymbol, _info.ExchangeName, CleanPrice(price), CleanVolume(volume), CleanSupply(supply), "0");
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
                return "";
            }
        }

        private string CleanPrice(string value)
        {
            var match = Regex.Match(value, @"\$(\d+(?:,\d{3})*(?:\.\d+)?)");
            if (match.Success)
                return match.Groups[1].Value.Replace(",", string.Empty).Trim();

            return value; 
        }

        private static string CleanSupply(string value)
        {
            var match = Regex.Match(value, @"\d+(?:,\d{3})*");
            if (match.Success)
                return match.Value.Replace(",", string.Empty);

            return value; 
        }

        private static string CleanVolume(string value)
        {
            var match = Regex.Match(value, @"\$(\d+(?:,\d{3})*)");
            if (match.Success)
                return match.Groups[1].Value.Replace(",", string.Empty).Trim();

            return value;
        }

        public void Dispose()
        {
            _driver.Dispose();
            _driver.Quit();
        }
    }
}
using Domain;
using Domain.Ports;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Infrastructure.Scraper
{
    public class CryptoScraperService : ICryptoScraperService, IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly ExchangeScrappingInfo _info;

        public CryptoScraperService(ExchangeScrappingInfo info, IWebDriver webDriver)
        {
            _info = info;
            _driver = webDriver;
        }
        ~CryptoScraperService()
        {

        }

        private void NavigateToUrl()
        {
            if (_driver?.Url != _info?.Url)
            {
                _driver?.Navigate().GoToUrl(_info?.Url);
            }
        }

        public CryptoData? GetCryptoInfoAsync()
        {
            NavigateToUrl(); 
            var price = !string.IsNullOrEmpty(_info.PriceXPath) ? ExtractData(_info.PriceXPath) : null;
            var volume =!string.IsNullOrEmpty(_info.Volume24HXPath) ? ExtractData(_info.Volume24HXPath) : null;
            var supply = !string.IsNullOrEmpty(_info.CirculatingSupplyXPath) ? ExtractData(_info.CirculatingSupplyXPath) : null;

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

        private string? CleanPrice(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            var match = Regex.Match(value, @"\$(\d+(?:,\d{3})*(?:\.\d+)?)");
            if (match.Success)
            {
                var res = match.Groups[1].Value.Replace(",", string.Empty).Trim();
                 return res;
            }
            return value; 
        }

        private static string? CleanSupply(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            var match = Regex.Match(value, @"\d+(?:,\d{3})*");
            if (match.Success)
                return match.Value.Replace(",", string.Empty);

            return value; 
        }

        private static string? CleanVolume(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            // Gère le cas des abréviations (M pour millions, B pour milliards, etc.)
            var abbreviationMatch = Regex.Match(value, @"\$(\d+(\.\d+)?)([MBK])", RegexOptions.IgnoreCase);
            if (abbreviationMatch.Success)
            {
                var numberValue = decimal.Parse(abbreviationMatch.Groups[1].Value, CultureInfo.InvariantCulture);
                var abbreviation = abbreviationMatch.Groups[3].Value.ToUpper();

                switch (abbreviation)
                {
                    case "K":
                        numberValue *= 1_000; // Multiplie par mille pour les milliers
                        break;
                    case "M":
                        numberValue *= 1_000_000; // Multiplie par un million pour les millions
                        break;
                    case "B":
                        numberValue *= 1_000_000_000; // Multiplie par un milliard pour les milliards
                        break;
                }

                return numberValue.ToString("0");
            }

            // Gère le cas où la valeur est en format standard (par exemple, "$5,221,833,130")
            var standardMatch = Regex.Match(value, @"\$(\d+(?:,\d{3})*)");
            if (standardMatch.Success)
                return standardMatch.Groups[1].Value.Replace(",", string.Empty).Trim();

            // Gère le cas où la valeur est un nombre simple ou précédé par un symbole du dollar
            var simpleNumberMatch = Regex.Match(value, @"^\$?(\d+)$");
            if (simpleNumberMatch.Success)
                return simpleNumberMatch.Groups[1].Value.Trim();

            return value;
        }



        public void Dispose()
        {            
            _driver.Quit();
        }

    }
}
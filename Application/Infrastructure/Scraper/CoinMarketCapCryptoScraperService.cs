using Domain;
using Domain.Ports;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Domain.Domain;
using System;

namespace Infrastructure.Scraper
{
    public class CoinMarketCapCryptoScraperService : ICryptoScraperService
    {
      
        private readonly ILogger<CoinMarketCapCryptoScraperService> _logger;
        private readonly ChromeDriver _driver;
        private readonly string _symbol;
        private readonly string _exchange;
        private readonly string _pairSymbol;
        private readonly string _url;

        private readonly ExchangeScrappingInfo _info;


        public CoinMarketCapCryptoScraperService(string symbol, string pairSymbol, string exchange, string url)
        {
            var options = new ChromeOptions();
            options.AddArgument("headless");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--disable-extensions");
            _driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), options, TimeSpan.FromMinutes(3));
            _symbol = symbol;
            _pairSymbol = pairSymbol;
            _exchange = exchange;
            _url = url;
            var factory = LoggerFactory.Create(builder => {
                builder.AddConsole();
            });

            _logger = factory.CreateLogger<CoinMarketCapCryptoScraperService>();


            _driver.Manage().Timeouts().PageLoad.Add(System.TimeSpan.FromSeconds(30));
        }
        public CoinMarketCapCryptoScraperService(ExchangeScrappingInfo info)
        {
            var options = new ChromeOptions();
            options.AddArgument("headless");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-gpu");
            _driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), options, TimeSpan.FromMinutes(3));
            var factory = LoggerFactory.Create(builder => {
                builder.AddConsole();
            });
            _info = info;

            _logger = factory.CreateLogger<CoinMarketCapCryptoScraperService>();
            _driver.Manage().Timeouts().PageLoad.Add(System.TimeSpan.FromSeconds(30));
            _symbol =info.CurrencySymbol;
            _pairSymbol = info.CurrencyPair;
            _exchange = info.ExchangeName;
            _url = info.Url;
        }

        public CryptoData? GetCryptoInfoAsync()
        {
            //_logger.LogInformation("Fetching data from {url}", _url);
            try
                {
             
           
                    NavigateToUrl();
                //_logger.LogInformation(_driver.PageSource);
                var price = ExtractData(_info.PriceXPath);
                //var price = "1";
                var volume24H = ExtractData(_info.Volume24HXPath);
                //var volume24H = "0";
                //var supply = "0";
                var supply = ExtractData(_info.CirculatingSupplyXPath);

                return new CryptoData(

                        _symbol,
                        _pairSymbol,
                        _symbol,
                        _exchange,
                        CleanPrice(price),
                        CleanVolume(volume24H),
                        CleanSupply(supply),
                        "0"
                    );
                }
                catch (Exception e)
                {   Console.WriteLine(e);
                    _logger.LogError(e, "An error occurred while fetching data.");
                    return new CryptoData();
                }

            
        }

        private void NavigateToUrl()
        { 
            if (_driver.Url != _url)
            {
                _driver.Navigate().GoToUrl(_url);
            }
            else
            {
                _driver.Navigate().Refresh();
            }
        }

        private string ExtractData(string xpath)
        {
            ;
            var wait = new WebDriverWait(_driver, _driver.Manage().Timeouts().AsynchronousJavaScript);
            var element = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.XPath(xpath)));
            return (string)((IJavaScriptExecutor)_driver).ExecuteScript("return arguments[0].innerText;", element);
        }

        private string CleanPrice(string value)
        {
            return Regex.Match(value, @"\$(\d+,\d+\.\d+)")?.Groups[1].Value.Replace("$", string.Empty).Trim() ?? throw new Exception("Value not found");
        }
        private static string CleanSupply(string btcSupply)
        {
            return Regex.Match(btcSupply, @"\d+,\d+,\d+")?.Value ?? throw new Exception("Value not found");

        }
        private static string CleanVolume(string volume24H)
        {
           return Regex.Match(volume24H, @"\$\d+,\d+,\d+,\d+")?.Value.Replace("$", string.Empty).Trim() ?? throw new Exception("Dollar value not found");
        }

        private static string BitCoinValue(string bitCoinValue)
        {
            var match = Regex.Match(bitCoinValue, @"\$(\d+,\d+\.\d+)");
            if (!match.Success) throw new Exception("Dollar value not found");
            bitCoinValue = match.Groups[1].Value;
            bitCoinValue = bitCoinValue.Replace("$", string.Empty).Trim();
            return bitCoinValue;
        }

        public void Dispose()
        {
            _driver.Dispose();
        }
    }

}
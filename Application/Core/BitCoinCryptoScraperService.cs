using Domain;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using PuppeteerSharp;
using System.Diagnostics;
using System.Globalization;
using OpenQA.Selenium.Support.UI;
using Microsoft.Extensions.Options;
using OpenQA.Selenium.Interactions;
using System.Text.RegularExpressions;

namespace Core
{
    public class BitCoinCryptoScraperService : ICryptoScraperService
    {
        private readonly HttpClient _httpClient = new();
        private readonly ChromeDriver _driver;
        private const string Url = "https://coinmarketcap.com/currencies/bitcoin/";

        public BitCoinCryptoScraperService()
        {
            var options = new ChromeOptions();
            options.AddArgument("headless");
            _driver = new ChromeDriver(options);
        }

        public CryptoData? GetCryptoInfoAsync()
        {
            CryptoData? cryptoData = new();
            try
            {
                if (_driver.Url != Url)
                {
                    _driver.Navigate().GoToUrl(Url);
                    //Console.WriteLine(_driver.PageSource);
                }
                else
                {
                    _driver.Navigate().Refresh();
                }

                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));


                var bitCoinValueElement = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.XPath("//*[@id=\"section-coin-overview\"]/div[1]/div[2]/span[2]")));
                var bitCoinValue = (string)((IJavaScriptExecutor)_driver).ExecuteScript("return arguments[0].innerText;", bitCoinValueElement);

                var match = Regex.Match(bitCoinValue, @"\$(\d+,\d+\.\d+)");
                if (!match.Success) throw new Exception("Dollar value not found");
                bitCoinValue = match.Groups[1].Value;
                bitCoinValue = bitCoinValue.Replace("$", string.Empty).Trim();



                var volume24HElement = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.XPath("//*[@id=\"section-coin-stats\"]/div/dl/div[2]/div[1]/dd")));
                var volume24H = (string)((IJavaScriptExecutor)_driver).ExecuteScript("return arguments[0].innerText;", volume24HElement);
                var volume24HMatch = Regex.Match(volume24H, @"\$\d+,\d+,\d+,\d+");
                if (!volume24HMatch.Success) throw new Exception("Dollar value not found");
                var volume24HValue = volume24HMatch.Value;
                volume24HValue = volume24HValue.Replace("$", string.Empty).Trim();


                var btcSupplyElement = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.XPath("//*[@id=\"section-coin-stats\"]/div/dl/div[5]/div/dd")
                ));
                var btcSupply = (string)((IJavaScriptExecutor)_driver).ExecuteScript("return arguments[0].innerText;", btcSupplyElement);
                var btcSupplyMatch = Regex.Match(btcSupply, @"\d+,\d+,\d+");
                if (!btcSupplyMatch.Success) throw new Exception("Dollar value not found");
                var btcSupplyValue = btcSupplyMatch.Value;


                cryptoData = new CryptoData
                {
                    Name = "Bitcoin",
                    Symbol = "BTC",
                    Price = bitCoinValue,
                    Volume24H = volume24HValue,
                    CirculatingSupply = btcSupplyValue,
                    TimeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                };

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception : " + e);
            }

            return cryptoData;
        }


        public Task<CryptoInfo> GetCryptoInfoAsync(string symbol)
        {
            throw new NotImplementedException();
        }

        //    url = "https://www.binance.com/en/trade/BTC_USDT";


        public async Task<CryptoInfo> GetCryptoInfoByApiAsync(string symbol)
        {
            var response = await _httpClient.GetStringAsync($"https://api.binance.com/api/v3/ticker/24hr?symbol={symbol}");
            var jObject = JObject.Parse(response);

            return new CryptoInfo
            {
                Name = symbol,
                BuyPrice = jObject["askPrice"].Value<decimal>(),
                Volume24h = jObject["volume"].Value<decimal>()
            };
        }

        public void Dispose()
        {
            _httpClient.Dispose();
            _driver.Dispose();
        }
    }

}
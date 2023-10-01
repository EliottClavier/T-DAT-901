using Domain;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using PuppeteerSharp;
using System.Diagnostics;
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
            //_httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
            var options = new ChromeOptions();
            options.AddArgument("headless");
            _driver = new ChromeDriver(options);
        }

        public  CryptoData GetCryptoInfoAsync()
        {
            try
            {
                if (_driver.Url != Url)
                {
                    _driver.Navigate().GoToUrl(Url);
                    Console.WriteLine(_driver.PageSource);
                }

                //_driver.Navigate().Refresh();
                //_driver.Navigate().Refresh();
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));

             
                //var bitCoinValueElement =  wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.XPath("//span[text()='BTC']/following-sibling::span")));
                var bitCoinValueElement =  wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.XPath("//*[@id=\"section-coin-overview\"]/div[1]/div[2]/span[2]")));
              
             
            


                var bitCoinValue = (string)((IJavaScriptExecutor)_driver).ExecuteScript("return arguments[0].innerText;", bitCoinValueElement);

                var match = Regex.Match(bitCoinValue, @"\$(\d+,\d+\.\d+)");
                if (match.Success)
                {
                    bitCoinValue = match.Groups[1].Value;
                    Console.WriteLine($"La valeur du Bitcoin est : ${bitCoinValue}");
                    bitCoinValue = bitCoinValue.Replace("$", string.Empty).Trim();
                }
                else
                {
                    Console.WriteLine("Valeur non trouvée");
                }



                var buyOrdersElement = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.XPath("//*[@id=\"section-coin-stats\"]/div/dl/div[2]/div[1]/dd")
                ));
                var buyOrders = (string)((IJavaScriptExecutor)_driver).ExecuteScript("return arguments[0].innerText;", buyOrdersElement);
               var dollarValueMatch = Regex.Match(buyOrders, @"\$\d+,\d+,\d+");


               if (dollarValueMatch.Success)
               {
                   var dollarValue = dollarValueMatch.Value;
                   Console.WriteLine($"Volume 24h : {dollarValue}");
               }
               else
               {
                   Console.WriteLine("Valeur en dollars non trouvée");
               }

               var btcVolumeElement = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.XPath("//*[@id=\"section-coin-stats\"]/div/dl/div[5]/div/dd")
               ));
                var btcVolume = (string)((IJavaScriptExecutor)_driver).ExecuteScript("return arguments[0].innerText;", btcVolumeElement);
               Console.WriteLine($"Volume BTC : {btcVolume}");


            }
            catch (Exception e)
            {
                Console.WriteLine("Exception : " + e);
            }

            return   new CryptoData();
        }
        //public async Task<CryptoData> GetCryptoInfoAsync(string date = "20221128/", int num = 10)
        //{
        //    var cryptoData = new CryptoData();
        //    //var url = $"https://coinmarketcap.com/historical/{date}";
        //    var url = $"https://coinmarketcap.com/currencies/bitcoin";


        //    var html = await _httpClient.GetStringAsync(url);

        //    var htmlDocument = new HtmlDocument();
        //    htmlDocument.LoadHtml(html);

        //    var rows = htmlDocument.DocumentNode.SelectNodes("//span[contains(@class, 'sc-16891c57-0 dxubiK base-text')]");

        //    for (int i = 0; i < num && i < rows.Count; i++)
        //    {
        //        var row = rows[i];

        //        var name = row.SelectSingleNode(".//td[contains(@class, 'cmc-table__cell--sort-by__name')]//a[contains(@class, 'cmc-link')]").InnerText.Trim();
        //        var marketCap = row.SelectSingleNode(".//td[contains(@class, 'cmc-table__cell--sort-by__market-cap')]").InnerText.Trim();
        //        var price = row.SelectSingleNode(".//td[contains(@class, 'cmc-table__cell--sort-by__price')]").InnerText.Trim();
        //        var circulatingSupplyAndSymbol = row.SelectSingleNode(".//td[contains(@class, 'cmc-table__cell--sort-by__circulating-supply')]").InnerText.Trim();
        //        var parts = circulatingSupplyAndSymbol.Split(' ');

        //        cryptoData.Name.Add(name);
        //        cryptoData.MarketCap.Add(marketCap);
        //        cryptoData.Price.Add(price);
        //        cryptoData.CirculatingSupply.Add(parts[0].Trim());
        //        cryptoData.Symbol.Add(parts[1].Trim());
        //    }

        //    return cryptoData;
        //}

        public Task<CryptoInfo> GetCryptoInfoAsync(string symbol)
        {
            throw new NotImplementedException();
        }

        //public async Task<CryptoInfo> GetCryptoInfoAsync(string symbol)
        //{
        //    try
        //    {
        //        var url = "https://coinmarketcap.com/currencies/bitcoin/";
        //        //var url = "https://www.kraken.com/prices/bitcoin";
        //        var response = await _httpClient.GetStringAsync(url);

        //        var htmlDocument = new HtmlDocument();
        //        htmlDocument.LoadHtml(response);

        //        Console.WriteLine(htmlDocument.Text);
        //        Console.WriteLine(htmlDocument.DocumentNode.Attributes);

        //        var priceNode = htmlDocument.DocumentNode.SelectSingleNode("//span[@class='asset-price']");

        //        if (priceNode == null)
        //        {
        //            throw new Exception("Price not found");
        //        }

        //        var price = decimal.Parse(priceNode.InnerText);

        //        return new CryptoInfo
        //        {
        //            Name = symbol,
        //            BuyPrice = price,

        //        };
        //    }
        //    catch(Exception ex )
        //    {
        //        Console.WriteLine(ex);
        //        return new CryptoInfo
        //        {
        //            Name = symbol,
        //            BuyPrice = 0,

        //        };

        //    }       
        //}

        //public async Task<CryptoInfo> GetCryptoInfoAsync(string? url)
        //{

        //    url = "https://www.binance.com/en/trade/BTC_USDT";


        //    await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        //    {
        //        Headless = true,
        //        ExecutablePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe"
        //    });
        //    await using var page = await browser.NewPageAsync();
        //    await page.GoToAsync(url);

        //    // Remplacez ces sélecteurs avec ceux appropriés pour la page spécifique que vous scrappez
        //    var priceSelector = ".showPrice"; // À remplacer
        //    //var volumeSelector = "#volume";// À remplacer

        //    var price = await page.QuerySelectorAsync(priceSelector);
        //    //var volume = await page.QuerySelectorAsync(volumeSelector);

        //    var cryptoInfo = new CryptoInfo
        //    {
        //        BuyPrice = decimal.Parse(await price.EvaluateFunctionAsync<string>("el => el.innerText")),
        //        //Volume24h = decimal.Parse(await volume.EvaluateFunctionAsync<string>("el => el.innerText"))
        //    };

        //    return cryptoInfo;
        //}

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
using Domain;
using Domain.Ports;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.DevTools;

namespace Infrastructure.Scraper
{
    public class BinanceScraperService : ICryptoScraperService
    {
        private readonly string _symbol;
        private readonly string _tradSymbol;
        private readonly string _exchange;
        private readonly string _url;
        private readonly ILogger _logger;

        public BinanceScraperService(string symbol, string tradSymbol, string exchange, string url, ILogger<BinanceScraperService> logger)
        {
            this._symbol = symbol;
            this._tradSymbol = tradSymbol;
            this._exchange = exchange;
            this._url = url;
            this._logger = logger;

        }

        public void Dispose()
        {

        }



        public  CryptoData? GetCryptoInfoAsync()
        {
         

                _logger.LogInformation("Fetching data from {url}", _url);
                return new CryptoData(_symbol, _tradSymbol, _symbol, _exchange, "0", "0", "0", "0");

           


        }

    }
}
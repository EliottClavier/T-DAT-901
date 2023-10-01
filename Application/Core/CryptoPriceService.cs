using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace Core
{
    public class CryptoPriceService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly ICryptoScraperService _scraperService;
        private readonly string _symbol;
        private readonly KafkaProducerService _kafkaProducer;

        public CryptoPriceService(ICryptoScraperService scraperService, KafkaProducerService kafkaProducer, string symbol = "BTCUSDT")
        {
            _scraperService = scraperService;
            _kafkaProducer = kafkaProducer;
            _symbol = symbol;
        }
        private async void FetchCryptoInfo(object state)
        {
            var info = await GetCryptoInfoAsync(_symbol);
            var jsonInfo = JsonConvert.SerializeObject(info);
            await _kafkaProducer.ProduceAsync("crypto-prices", jsonInfo); 

            //Console.WriteLine($"Name: {info.Name}, Buy Price: {info.BuyPrice}, 24h Volume: {info.Volume24h}");
        }
        public Task<CryptoData?> GetCryptoInfoAsync(string symbol)
        {
            return Task.FromResult( _scraperService.GetCryptoInfoAsync());
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork,
                null, TimeSpan.Zero, TimeSpan.FromSeconds(2));

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            var info = await GetCryptoInfoAsync(_symbol);
            Console.WriteLine($"Name: {info.Name}, Buy Price: {info.Price}, 24h Volume: {info.Volume24H} Supply: {info.CirculatingSupply} TimeStamp: {info.TimeStamp}");
          
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }

}

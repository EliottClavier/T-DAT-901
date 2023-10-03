using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Domain.Ports;
using Infrastructure.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Core
{
    public class CryptoHostedService : IHostedService, IDisposable
    {
        private readonly List<Timer> _timers = new();
        private readonly List<ICryptoScraperService> _scraperServices;
        private readonly KafkaProducerService _kafkaProducer;
        private readonly ILogger<CryptoHostedService> _logger;
        private readonly object _lock = new object();

        public CryptoHostedService( KafkaProducerService kafkaProducer, Func<string, ICryptoScraperService>? serviceFactory, ILogger<CryptoHostedService> logger)
        {
           
            _kafkaProducer = kafkaProducer;
            _logger = logger;

            if (serviceFactory == null) return;
            _scraperServices = new List<ICryptoScraperService>
            {
                serviceFactory("CMC-BTC"),
                //serviceFactory("CMC-ETH"),
                //serviceFactory("BNB-BTC"),
                //serviceFactory("BNB-ETH")
            };
        }
        private void FetchCryptoInfo(ICryptoScraperService service)
        {
            try
            {
                //if (Monitor.TryEnter(_lock))
                {
                    var info = service.GetCryptoInfoAsync();
                    var jsonInfo = JsonConvert.SerializeObject(info);
                    //await _kafkaProducer.ProduceAsync("bitcoin-infos", jsonInfo);

                    _logger.LogInformation(
                        $"Name: {info.CurrencyName}, Buy Price: {info.Price}, 24h Volume: {info.Volume24H} Supply: {info.CirculatingSupply} TimeStamp: {info.TimeStamp}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching crypto info.");
            }
            finally
            {
                //Monitor.Exit(_lock);
            }
        }
  

        public Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var service in _scraperServices)
            {
                void Callback(object? _) =>  FetchCryptoInfo(service);

                var timer = new Timer(Callback,
                    null, TimeSpan.Zero, TimeSpan.FromSeconds(2));

                _timers.Add(timer);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var timer in _timers)
            {
                timer?.Change(Timeout.Infinite, 0);
            }
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            foreach (var timer in _timers)
            {
                timer?.Dispose();
            }
        }
    }

}

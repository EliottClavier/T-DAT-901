﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application;
using Domain;
using Domain.Domain;
using Domain.Ports;
using Infrastructure.Kafka;
using Microsoft.Extensions.Configuration;
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

        public CryptoHostedService(KafkaProducerService kafkaProducer,
            Func<ExchangeScrappingInfo, ICryptoScraperService>? serviceFactory,
            IExchangeScrappingInfoProvider exchangeScrappingInfoProvider,
            ILogger<CryptoHostedService> logger)
        {

            _kafkaProducer = kafkaProducer;
            _logger = logger;

            var cryptoList = exchangeScrappingInfoProvider.GetExchangeScrappingInfo();
            if (serviceFactory == null) return;

            _scraperServices = new List<ICryptoScraperService>();
            foreach (var crypto in cryptoList)
            {
                _scraperServices.Add( serviceFactory(crypto));
            };
        }

        private void FetchCryptoInfo(ICryptoScraperService service)
        {
            try
            {
                {
                    var info = service.GetCryptoInfoAsync();

                    var jsonInfo = JsonConvert.SerializeObject(info);
                    //await _kafkaProducer.ProduceAsync("bitcoin-infos", jsonInfo);

                    _logger.LogInformation(
                       $"Exchange: {info?.ExchangeName}, Name: {info?.CurrencyName}, Price: {info?.Price}, 24h Volume: {info?.Volume24H} Supply: {info?.CirculatingSupply} TimeStamp: {info?.TimeStamp}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching crypto info.");
            }
            finally
            {
            }
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {

            foreach (var service in _scraperServices)
            {
                void Callback(object? _) => FetchCryptoInfo(service);

                var timer = new Timer(Callback,
                    null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

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
            StopAsync(CancellationToken.None);

            foreach (var timer in _timers)
            {
                timer?.Dispose();
            }
        }
    }

}
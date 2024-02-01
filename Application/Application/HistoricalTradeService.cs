using Infrastructure.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application
{
    public class HistoricalTradeService :  IHostedService, IDisposable
    {
        private IOptions<KafkaSettings> _kafkaSettings;
        private KafkaProducerService _kafkaProducerService;
        private ILogger _logger;

        private SemaphoreSlim _semaphore;

        public HistoricalTradeService(IOptions<KafkaSettings> kafkaSettings, KafkaProducerService kafkaProducer, ILogger<HistoricalTradeService> logger)
        {
            _kafkaSettings = kafkaSettings;
            _kafkaProducerService = kafkaProducer;
            _logger = logger;

            _semaphore = new(5);
     
        }

        public void Dispose()
        {
            
            _semaphore.Release();
            _semaphore.Dispose();

        }

        public async Task GetHistoricalTrades()
        {
            _logger.LogInformation("GetHistoricalTrades");
           
            var loader = new TradeLoader(_kafkaProducerService);
        
            var patternCsv = "*.csv";

            var csvFolder = "/app/app-data/trades/aggTrades";
            var filePaths = Directory.GetFiles(csvFolder, patternCsv);
            _logger.LogInformation($"Found {filePaths.Length} files");


         
            var tasks = filePaths.Select(async filePath =>
            {
                // Attendre qu'un slot se libère.
                await _semaphore.WaitAsync();

                try
                {
                    await loader.LoadTradesFromCsvAndSendToKafka(filePath, _kafkaSettings.Value.TransactionTopic);
                }
                finally
                {
                    // Libérer le slot une fois la tâche terminée.
                    _semaphore.Release();
                }
            });

            // Attendre que toutes les tâches soient terminées.
            await Task.WhenAll(tasks);

        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.Run(async () =>            {
            
                    await Task.Delay(10000);
                    await GetHistoricalTrades();
                
            }, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Dispose();
            return Task.CompletedTask;          
        }


    }
}


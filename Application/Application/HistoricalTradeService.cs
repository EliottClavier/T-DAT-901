using Infrastructure.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application
{
    public class HistoricalTradeService
    {
        private IOptions<KafkaSettings> _kafkaSettings;
        private KafkaProducerService _kafkaProducerService;
        private ILogger _logger;
        
        private List<Task> _tasks = new();
        private SemaphoreSlim _semaphore = new (6);

        public HistoricalTradeService(IOptions<KafkaSettings> kafkaSettings, KafkaProducerService kafkaProducer, ILogger<HistoricalTradeService> logger)
        {
            _kafkaSettings = kafkaSettings;
            _kafkaProducerService = kafkaProducer;
            _logger = logger;
       


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
    }
}


using Infrastructure.Kafka;
using Microsoft.Extensions.Options;

namespace Application
{
    public class HistoricalTradeService
    {
        private IOptions<KafkaSettings> _kafkaSettings;
        private KafkaProducerService _kafkaProducerService;

        private List<Task> _tasks = new();
        private SemaphoreSlim _semaphore = new (6);

        public HistoricalTradeService(IOptions<KafkaSettings> kafkaSettings, KafkaProducerService kafkaProducer)
        {
            _kafkaSettings = kafkaSettings;
            _kafkaProducerService = kafkaProducer;
        }

        public async Task GetHistoricalTrades()
        {
            var loader = new TradeLoader(_kafkaProducerService);
            var csvFolder = "..\\Data\\Trades\\aggTrades\\";
            var patternCsv = "*.csv";
            var filePaths = Directory.GetFiles(csvFolder, patternCsv);

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


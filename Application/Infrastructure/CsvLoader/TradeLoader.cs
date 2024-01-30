using Infrastructure.Kafka;
using CsvHelper;
using Newtonsoft.Json;
using System.Globalization;
using Domain;
using Infrastructure.CsvLoader;
using CsvHelper.Configuration;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.Logging;
using Confluent.Kafka;

public class TradeLoader
{
    private readonly KafkaProducerService _kafkaProducerService;
    private readonly ILogger _logger;


    private ulong _numberOfbatch = 0;
    public TradeLoader(KafkaProducerService kafkaProducerService)
    {
        _kafkaProducerService = kafkaProducerService;
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .AddConsole();
        });

        _logger = loggerFactory.CreateLogger<TradeLoader>();

    }

    public async Task LoadTradesFromCsvAndSendToKafka(string filePath, string topic)
    {
        try
        {
            var batchSize = 100;
            var batch = new List<CryptoTrade>();

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string[] parts = fileName.Split('-');
            string symbol = parts[0];

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                BufferSize = 8192,
                HasHeaderRecord = false,
                IgnoreBlankLines = true,
                DetectColumnCountChanges = false
            }))
            {
                csv.Context.RegisterClassMap(new TradeMapping(symbol));

                foreach (var trade in csv.GetRecords<CryptoTrade>())
                {
                    batch.Add(trade);

                    if (batch.Count >= batchSize)
                    {
                        await SendBatchToKafka(batch, topic);
                        batch.Clear();
                    }
                }
                if (batch.Count > 0)
                {
                    await SendBatchToKafka(batch, topic);
                }
            }

        }
        catch (CsvHelperException ex)
        {
            Console.WriteLine($"Erreur lors de la lecture du CSV : {ex.Message}");
        }

    }
    private async Task SendBatchToKafka(List<CryptoTrade> batch, string topic)
    {
        var count = (ulong)batch.Count;
        _numberOfbatch += count;
        var batchJson = JsonConvert.SerializeObject(batch);
        
        _logger.LogInformation(_numberOfbatch.ToString());
        _logger.LogInformation(" ");
        _logger.LogInformation(batchJson);

        await _kafkaProducerService.ProduceAsync(batchJson, topic);
    }
}

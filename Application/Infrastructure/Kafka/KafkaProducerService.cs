using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static Confluent.Kafka.ConfigPropertyNames;

namespace Infrastructure.Kafka;

public class KafkaProducerService : IDisposable
{
    private readonly ProducerConfig _config;
    private string _bootstrapServers;
    private readonly string _topic;
    private readonly ILogger<KafkaProducerService> _logger;
    private IProducer<Null, string>? producer;

    public KafkaProducerService(IOptions<KafkaSettings> kafkaSettings, ILogger<KafkaProducerService> logger)
    {
        _logger = logger;
        _logger.LogInformation("KafkaProducerService");
        _logger.LogInformation(kafkaSettings.Value.BootstrapServers);
        _logger.LogInformation(kafkaSettings.Value.DefaultTopic);
        _bootstrapServers = kafkaSettings.Value.BootstrapServers;
        _topic = kafkaSettings.Value.DefaultTopic;
        _config = new ProducerConfig { BootstrapServers = _bootstrapServers };
    }

    public async Task ProduceAsync(string message, string? topic = null)
    {
        try
        {
            topic ??= _topic;
            producer = new ProducerBuilder<Null, string>(_config).Build();
          
            await producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    public  void Produce(string message, string? topic = null)
    {
        try
        {
            topic ??= _topic;
            producer = new ProducerBuilder<Null, string>(_config).Build();

             producer.Produce(topic, new Message<Null, string> { Value = message });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public void Dispose()
    {
        producer?.Dispose();
    }
}
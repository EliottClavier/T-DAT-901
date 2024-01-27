using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
        _bootstrapServers = kafkaSettings.Value.BootstrapServers;
        _topic = kafkaSettings.Value.DefaultTopic;
        _config = new ProducerConfig { BootstrapServers = _bootstrapServers };

        producer = new ProducerBuilder<Null, string>(_config).Build();
    }

    public async Task ProduceAsync(string message, string? topic = null)
    {
        try
        {
            topic ??= _topic;
            await producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la production du message Kafka");
        }
    }

    public void Produce(string message, string? topic = null)
    {
        try
        {
            topic ??= _topic;
            producer.Produce(topic, new Message<Null, string> { Value = message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la production du message Kafka");
        }
    }

    public void Dispose()
    {
        producer?.Dispose();
    }
}
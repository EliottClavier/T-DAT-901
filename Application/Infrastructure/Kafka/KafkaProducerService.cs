using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace Infrastructure.Kafka;

public class KafkaProducerService
{
    private readonly ProducerConfig _config;
    private string _bootstrapServers;
    private readonly string _topic;

    public KafkaProducerService(IOptions<KafkaSettings> kafkaSettings)
    {
        _bootstrapServers = kafkaSettings.Value.BootstrapServers;
        _topic = kafkaSettings.Value.DefaultTopic;
        _config = new ProducerConfig { BootstrapServers = _bootstrapServers };
    }

    public async Task ProduceAsync(string message, string? topic)
    {
        try
        {
            topic ??= _topic;
            using var producer = new ProducerBuilder<Null, string>(_config).Build();
            await producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

    }
}
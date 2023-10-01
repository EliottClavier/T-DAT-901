using Confluent.Kafka;

namespace Core;

public class KafkaProducerService
{
    private readonly ProducerConfig _config;

    public KafkaProducerService(string bootstrapServers)
    {
        _config = new ProducerConfig { BootstrapServers = bootstrapServers };
    }

    public async Task ProduceAsync(string topic, string message)
    {
        try
        {
            using var producer = new ProducerBuilder<Null, string>(_config).Build();
            await producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);  
        }
     
    }
}
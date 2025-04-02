using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Yaba.Infrastructure.DTO;
using Yaba.Infrastructure.Kafka.Helpers;

namespace Yaba.Infrastructure.Kafka.Services;

public class ProducerService
{
    private KafkaConfig _kafkaConfig;

    public ProducerService(IOptions<KafkaConfig> option)
    {
        _kafkaConfig = option.Value;
    }

    public async Task ProduceAsync(string transcationId, string categoryId, CancellationToken cancellationToken)
    {
        string topic = KafkaTopics.CategoryTopic;

        var config = new ProducerConfig
        {
            BootstrapServers = _kafkaConfig.BootstrapServers,
            AllowAutoCreateTopics = true,
            Acks = Acks.All
        };

        using var producer = new ProducerBuilder<string, string>(config).Build();

        try
        {
            await producer.ProduceAsync(topic, 
                new Message<string, string> { Key = transcationId, Value = categoryId }
                , cancellationToken);
        }
        catch (Exception ex)
        {
            throw;
        }
        finally
        {
            producer.Flush(cancellationToken);
        }

    }
}

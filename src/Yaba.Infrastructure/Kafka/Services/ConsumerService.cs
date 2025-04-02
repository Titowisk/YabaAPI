using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Yaba.Infrastructure.DTO;
using Yaba.Infrastructure.Kafka.Helpers;

namespace Yaba.Infrastructure.Kafka.Services;
public class ConsumerService
{
    private readonly KafkaConfig _kafkaConfig;
    private readonly ILogger<ConsumerService> _logger;

    public ConsumerService(IOptions<KafkaConfig> option, ILogger<ConsumerService> logger)
    {
        _kafkaConfig = option.Value;
        _logger = logger;
    }

    public Task ConsumeAsync(CancellationToken cancellationToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _kafkaConfig.BootstrapServers,
            GroupId = "categorize-transactions-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();

        consumer.Subscribe(KafkaTopics.CategoryTopic);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var consumeResult = consumer.Consume(cancellationToken);
                if (consumeResult.IsPartitionEOF)
                {
                    continue;
                }
                var message = consumeResult.Message;
                _logger.LogInformation("Consumed message: key {key} value {value} at offset: {offset}", 
                    message.Key, 
                    message.Value, 
                    consumeResult.TopicPartitionOffset);
            }
        }
        catch (Exception ex)
        {
        }

       return Task.CompletedTask;
    }
}

using Yaba.Infrastructure.Kafka.Services;

namespace Yaba.Worker.Categorizer;

public class KafkaWorker : BackgroundService
{
    private readonly ILogger<KafkaWorker> _logger;
    private readonly ConsumerService _consumerService;

    public KafkaWorker(ILogger<KafkaWorker> logger, ConsumerService consumerService)
    {
        _logger = logger;
        _consumerService = consumerService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Started worker running at: {time}", DateTimeOffset.Now);

        while (!stoppingToken.IsCancellationRequested)
        {
            await _consumerService.ConsumeAsync(stoppingToken);
        }
    }
}

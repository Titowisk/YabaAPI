namespace Yaba.Worker.Categorizer;

public class KafkaWorker : BackgroundService
{
    private readonly ILogger<KafkaWorker> _logger;

    public KafkaWorker(ILogger<KafkaWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
            await Task.Delay(1000, stoppingToken);
        }
    }
}

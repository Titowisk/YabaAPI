using Azure.Storage.Queues;
using Microsoft.Extensions.Options;
using Yaba.Infrastructure.AzureStorageQueue.Contracts;
using Yaba.Infrastructure.DTO;

namespace Yaba.Infrastructure.AzureStorageQueue.Implementations
{
    public class QueueMessageService : IQueueMessageService
    {
        private readonly AzureConfig _azureConfig;
        private readonly ConnectionStrings _connectionStrings;

        public QueueMessageService(IOptions<AzureConfig> azureConfig, IOptions<ConnectionStrings> connectionStrings)
        {
            _azureConfig = azureConfig.Value;
            _connectionStrings = connectionStrings.Value;
        }

        public void SendMessage(string message)
        {
            // Instantiate a QueueClient which will be used to create and manipulate the queue
            QueueClient queueClient = new QueueClient(_connectionStrings.AzureWebJobsStorage, _azureConfig.QueueName);

            if (queueClient.Exists())
            {
                // Send a message to the queue
                queueClient.SendMessage(message);
            }
        }
    }
}

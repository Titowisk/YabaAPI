using Azure.Storage.Queues;
using Microsoft.Extensions.Options;
using Yaba.Infrastructure.AzureStorageQueue.Contracts;
using Yaba.Infrastructure.DTO;

namespace Yaba.Infrastructure.AzureStorageQueue.Implementations
{
    public class QueueMessageService : IQueueMessageService
    {
        private readonly AzureConfig _option;

        public QueueMessageService(IOptions<AzureConfig> option)
        {
            _option = option.Value;
        }

        public void SendMessage(string message)
        {
            // Instantiate a QueueClient which will be used to create and manipulate the queue
            QueueClient queueClient = new QueueClient(_option.AzureWebJobsStorage, _option.QueueName);

            if (queueClient.Exists())
            {
                // Send a message to the queue
                queueClient.SendMessage(message);
            }
        }
    }
}

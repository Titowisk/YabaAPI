using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Yaba.Infrastructure.AzureStorageQueue.Contracts;

namespace Yaba.Infrastructure.AzureStorageQueue.Implementations
{
    public class QueueMessageService : IQueueMessageService
    {
        public Task SendCategorizeTransactionsMessage(string message)
        {
            throw new NotImplementedException();
        }
    }
}

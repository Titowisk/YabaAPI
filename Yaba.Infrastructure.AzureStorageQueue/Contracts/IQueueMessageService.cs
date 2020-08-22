using System.Threading.Tasks;

namespace Yaba.Infrastructure.AzureStorageQueue.Contracts
{
    public interface IQueueMessageService
    {
        Task SendCategorizeTransactionsMessage(string message);
    }
}

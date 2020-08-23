using System.Threading.Tasks;

namespace Yaba.Infrastructure.AzureStorageQueue.Contracts
{
    public interface IQueueMessageService
    {
        void SendMessage(string message);
    }
}

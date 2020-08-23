using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Yaba.Application.TransactionServices;

namespace Yaba.WebJob
{
    public class Functions
    {
        private readonly ITransactionService _transactionService;

        public Functions(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        public void ProcessQueueMessage([QueueTrigger("yabadev")] string message, ILogger logger)
        {
            _transactionService.GetType();
            logger.LogInformation(message);
        }

        //public static void ProcessQueueMessage([QueueTrigger("yabadev")] string message, ILogger logger)
        //{
        //    logger.LogInformation(message);
        //}
    }
}

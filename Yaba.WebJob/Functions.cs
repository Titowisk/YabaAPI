using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Yaba.Application.TransactionServices;
using Yaba.Tools.Validations;

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
            try
            {
                Validate.IsTrue(long.TryParse(message, out long transactionId), "Invalid transaction from queue");
                _transactionService.CategorizeAllOtherTransactions(transactionId).Wait();
            }
            catch (System.Exception)
            {
                logger.LogInformation(message);
            }
        }
    }
}

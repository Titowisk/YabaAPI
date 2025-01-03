using MassTransit;
using Yaba.Application.TransactionServices;
using Yaba.Domain.Models.Transactions.MessageContracts;

namespace Yaba.TransactionCategoryWorker.Consumers
{
    public class UpdateTransactionsCategoryConsumer : IConsumer<UpdateTransactionsCategory>
    {
        private readonly ITransactionService _transactionService;
        private readonly ILogger<UpdateTransactionsCategoryConsumer> _logger;

        public UpdateTransactionsCategoryConsumer(ITransactionService transactionService,
            ILogger<UpdateTransactionsCategoryConsumer> logger)
        {
            _transactionService = transactionService;
            _logger = logger;
        }

        // TODO: not working, study more about exchanges and queues of RabbitMQ
        public async Task Consume(ConsumeContext<UpdateTransactionsCategory> context)
        {
            try
            {
                await _transactionService.CategorizeAllOtherTransactions(context.Message.TransactionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UpdateTransactionsCategoryConsumer");
            }
        }
    }
}

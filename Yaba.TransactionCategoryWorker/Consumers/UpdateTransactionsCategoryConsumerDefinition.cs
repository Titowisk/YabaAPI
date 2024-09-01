using MassTransit;


namespace Yaba.TransactionCategoryWorker.Consumers
{
    public class UpdateTransactionsCategoryConsumerDefinition: ConsumerDefinition<UpdateTransactionsCategoryConsumer>
    {
        public UpdateTransactionsCategoryConsumerDefinition()
        {
            // TODO: test EfCore against concurrent calls
            ConcurrentMessageLimit = 1;
        }
    }
}

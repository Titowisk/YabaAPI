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

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<UpdateTransactionsCategoryConsumer> consumerConfigurator)
        {
            // TODO: should I keept it?
            endpointConfigurator.UseMessageRetry(r => r.Interval(5, 1000));
        }
    }
}

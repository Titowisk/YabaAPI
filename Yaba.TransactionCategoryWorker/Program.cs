using MassTransit;
using Yaba.Domain.Models.Transactions.MessageContracts;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddMassTransit(configure =>
{
    // https://masstransit.io/documentation/transports/rabbitmq
    configure.UsingRabbitMq((context, configuration) =>
    {
        configuration.ReceiveEndpoint("transaction-categorization-queue", configureEndpoint =>
        {
            configureEndpoint.Bind("api-worker-exchange");
            configureEndpoint.Bind<UpdateTransactionsCategory>();
        });

        configuration.Host("localhost", "/", h => {
            h.Username("guest");
            h.Password("guest");
        });

        configuration.ConfigureEndpoints(context);
    });
});

var host = builder.Build();
host.Run();

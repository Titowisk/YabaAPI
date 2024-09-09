using MassTransit;
using Yaba.Domain.Models.Transactions.MessageContracts;
using Yaba.Infrastructure.DTO;
using Yaba.Infrastructure.IoC;
using Yaba.TransactionCategoryWorker.Consumers;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddMassTransit(configure =>
{
    configure.AddConsumer<UpdateTransactionsCategoryConsumer>();

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

builder.Services
        .Configure<ConnectionStrings>(builder.Configuration.GetSection("ConnectionStrings"));


DependencyResolver.RegisterServices(builder.Services, builder.Configuration);

var host = builder.Build();
host.Run();

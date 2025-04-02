using Yaba.Infrastructure.DTO;
using Yaba.Infrastructure.IoC;
using Yaba.Worker.Categorizer;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<KafkaWorker>();

builder.Services
    .Configure<ConnectionStrings>(builder.Configuration.GetSection("ConnectionStrings"));
builder.Services
    .Configure<KafkaConfig>(builder.Configuration.GetSection("KafkaConfig"));

DependencyResolver.RegisterServices(builder.Services, builder.Configuration);

var host = builder.Build();
host.Run();

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Yaba.WebJob
{
    public class Program
    {
        static async Task Main()
        {
            var builder = new HostBuilder();
            builder
            .UseEnvironment("Development")
            .ConfigureWebJobs(b =>
            {
                b.AddAzureStorageCoreServices();
                b.AddAzureStorage();
            })
            .ConfigureLogging((context, b) =>
            {
                b.AddConsole();
            })
            .ConfigureServices(b =>
            {
                IServiceCollection services = new ServiceCollection();
                var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

                Infrastructure.IoC.DependencyResolver.RegisterServices(services, configuration);
            });

            var host = builder.Build();
            using (host)
            {
                await host.RunAsync();
            }
        }
    }
}

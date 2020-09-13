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
            .ConfigureAppConfiguration((context, b) =>
            {
                if (context.HostingEnvironment.IsDevelopment())
                {
                    b.AddUserSecrets<Program>();
                }
            })
            .ConfigureWebJobs(b =>
            {
                b.AddAzureStorageCoreServices();
                b.AddAzureStorage();
            })
            .ConfigureLogging((context, b) =>
            {
                b.AddConsole();
            })
            .ConfigureServices((context, b) =>
            {
                Infrastructure.IoC.DependencyResolver.RegisterServices(b, context.Configuration);
            });

            var host = builder.Build();
            using (host)
            {
                await host.RunAsync();
            }
        }
    }
}

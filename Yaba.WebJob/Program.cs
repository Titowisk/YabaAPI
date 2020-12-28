using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Yaba.WebJob
{
    public class Program
    {
        static async Task Main()
        {
            // $Env:ASPNETCORE_ENVIRONMENT = "Development"
            var builder = new HostBuilder();
            builder
            .UseEnvironment(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
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

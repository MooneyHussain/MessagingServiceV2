using MessagingService.HostedService.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace MessagingService.HostedService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder()
                .UseConsoleLifetime()
                .Build();

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder()
        {
            return new HostBuilder()
                .ConfigureAppConfiguration(config => { config.AddJsonFile("appsettings.json"); })
                .ConfigureServices((hostingContext, services) =>
                {
                    services.AddTransient<ILogger, ConsoleLogger>();
                    services.AddHostedService<QueuedMessageBackgroundTask>();

                    services.Configure<ServiceBusConnectionSettings>(
                        hostingContext.Configuration.GetSection("ServiceBusConnectionSettings"));
                });
        }
    }
}

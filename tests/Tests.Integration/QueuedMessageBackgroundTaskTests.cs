using MessagingService.HostedService;
using MessagingService.HostedService.Configuration;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Integration
{
    public class QueuedMessageBackgroundTaskTests
    {
        static IQueueClient queueClient;
        readonly IHost host;
        readonly Mock<ILogger> logger = new Mock<ILogger>();

        const string TestConnectionString = "-- swap for connection string used for testing purposes --";
        const string TestQueueName = "-- swap for queue used for testing purposes --";

        public QueuedMessageBackgroundTaskTests()
        {
            var builder = Program.CreateHostBuilder();

            builder.ConfigureServices((hostingContext, services) =>
            {
                services.AddSingleton(logger.Object);

                // overriding configuration
                services.Configure<ServiceBusConnectionSettings>(
                    settings =>
                    {
                        settings.ConnectionString = TestConnectionString;
                        settings.QueueName = TestQueueName;
                    });
            });

            host = builder.Build();
            queueClient = new QueueClient(TestConnectionString, TestQueueName);
        }

        [Fact]
        public async Task ExecuteAsync_CompleteMessageAsync()
        {
            var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var message = new Message
            {
                MessageId = Guid.NewGuid().ToString()
            };

            queueClient = new QueueClient(TestConnectionString, TestQueueName);

            await queueClient.SendAsync(message);

            await host.RunAsync(cancellationToken.Token);

            logger.Verify(l => l.LogEvent(It.Is<string>(m => m.Contains(message.MessageId))));
        }
    }
}

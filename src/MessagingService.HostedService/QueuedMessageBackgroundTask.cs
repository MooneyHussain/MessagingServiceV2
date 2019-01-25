using MessagingService.HostedService.Configuration;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace MessagingService.HostedService
{
    public class QueuedMessageBackgroundTask : BackgroundService
    {
        static IQueueClient queueClient;
        readonly ILogger logger;

        public QueuedMessageBackgroundTask(
            IOptionsMonitor<ServiceBusConnectionSettings> settings,
            ILogger logger)
        {
            queueClient = new QueueClient(settings.CurrentValue.ConnectionString, settings.CurrentValue.QueueName);
            this.logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            queueClient.RegisterMessageHandler(HandleAsync, new MessageHandlerOptions(HandleError)
            {
                AutoComplete = false
            });

            return Task.CompletedTask;
        }

        async Task HandleAsync(Message message, CancellationToken cancellationToken)
        {
            logger.LogEvent($"...Handling: {message.MessageId}");

            await queueClient.CompleteAsync(message.SystemProperties.LockToken);

            logger.LogEvent($"Completed: {message.MessageId}");
        }

        Task HandleError(ExceptionReceivedEventArgs args)
        {
            logger.LogEvent($"Error: {args.Exception.Message}");

            return Task.CompletedTask;
        }
    }
}

namespace MessagingService.HostedService.Configuration
{
    public class ServiceBusConnectionSettings
    {
        public string ConnectionString { get; set; }
        public string QueueName { get; set; }
    }
}

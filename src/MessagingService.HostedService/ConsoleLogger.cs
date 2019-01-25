using System;

namespace MessagingService.HostedService
{
    public class ConsoleLogger : ILogger
    {
        public void LogEvent(string eventMessage) => Console.WriteLine(eventMessage);
    }

    public interface ILogger
    {
        void LogEvent(string eventMessage);
    }
}

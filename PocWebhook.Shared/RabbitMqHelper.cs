using RabbitMQ.Client;
using System.Net.Http.Headers;

namespace PocWebhook.Shared
{
    public static class RabbitMqHelper
    {
        public const string queueName = "webhook_queue";

        public static IConnection GetRabbitMqConnection()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost", 
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };

            return factory.CreateConnection();
        }
    }
}

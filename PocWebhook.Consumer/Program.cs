// See https://aka.ms/new-console-template for more information
using PocWebhook.Shared;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net.Http.Headers;
using System.Text;


Console.WriteLine($"{DateTime.Now} - Consumer started.");
while (true)
{
    Consume();
    Task.Delay(5000).Wait();
}

static void Consume()
{
    using (var connection = RabbitMqHelper.GetRabbitMqConnection())
    using (var channel = connection.CreateModel())
    {
        channel.QueueDeclare(queue: RabbitMqHelper.queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            string message = Encoding.UTF8.GetString(ea.Body.ToArray());
            Console.WriteLine($"{DateTime.Now} - Message read.");
            bool result = CallWebhook(message);

            if (result)
            {
                channel.BasicAck(ea.DeliveryTag, false);
                Console.WriteLine($"{DateTime.Now} - Message: {message} consumed.");
            }
        };

        var consumerTag = channel.BasicConsume(queue: RabbitMqHelper.queueName, autoAck: false, consumer: consumer);

        Console.WriteLine("Press any key to exit.");
        Console.ReadLine();
    }
}

static bool CallWebhook(string message)
{
    using (var httpClient = new HttpClient())
    {
        try
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return false;
            }

            var apiUrl = "https://localhost:7174/api/crm/addusertoemaillist";
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var content = new StringContent(message, Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync(apiUrl, content).Result;

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"{DateTime.Now} - Webhook call successful.");
            }
            else
            {
                Console.WriteLine($"{DateTime.Now} - Webhook call failed. Status Code: {response.StatusCode}, Reason: {response.ReasonPhrase}.");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception occurred during webhook call: {ex.Message}.");
            return false;
        }
    }

    return true;
}
using Microsoft.AspNetCore.Mvc;
using PocWebhook.Shared;
using System.Text;
using System.Text.Json;

namespace PocWebHook.ApiProducer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromBody] UserDTO userDTO)
        {
            using (var connection = RabbitMqHelper.GetRabbitMqConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: RabbitMqHelper.queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(userDTO));

                channel.BasicPublish(exchange: "", routingKey: RabbitMqHelper.queueName, mandatory: false, basicProperties: null, body: body);
            }

            return Ok("Message published to RabbitMQ");
        }
    }
}

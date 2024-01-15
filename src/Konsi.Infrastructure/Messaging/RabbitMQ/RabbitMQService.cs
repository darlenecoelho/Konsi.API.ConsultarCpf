using Konsi.Domain.Interfaces;
using Konsi.Infrastructure.Messaging.Configuration;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;

namespace Konsi.Infrastructure.Messaging.RabbitMQ;

public class RabbitMQService : IMessageQueueService
{
    private readonly RabbitMQSettings _settings;

    public RabbitMQService(IOptions<RabbitMQSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task PublishCpfAsync(string cpf)
    {
        var factory = new ConnectionFactory()
        {
            HostName = _settings.HostName,
            UserName = _settings.UserName,
            Password = _settings.Password
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: _settings.QueueName,
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        var body = Encoding.UTF8.GetBytes(cpf);

        channel.BasicPublish(exchange: "",
                             routingKey: _settings.QueueName,
                            basicProperties: null,body: body);
    }
}


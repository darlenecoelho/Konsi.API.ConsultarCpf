using Konsi.Domain.Interfaces;
using Konsi.Infrastructure.Messaging.Configuration;
using Konsi.Infrastructure.Redis.Data;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;

namespace Konsi.Infrastructure.Messaging.RabbitMQ;

public class RabbitMQService : IMessageQueueService
{
    private readonly RabbitMQSettings _settings;
    private readonly CacheService _cacheService;

    public RabbitMQService(IOptions<RabbitMQSettings> settings, CacheService cacheService)
    {
        _settings = settings.Value;
        _cacheService = cacheService;
    }

    public async Task PublishCpfAsync(string cpf)
    {
        string cachedData = await _cacheService.GetCachedDataAsync(cpf);
        if (!string.IsNullOrEmpty(cachedData))
        {
            // Se os dados já estiverem no cache, não é necessário publicar na fila
            return;
        }

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
                            basicProperties: null, body: body);
    }
}
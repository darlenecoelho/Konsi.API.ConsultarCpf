using Konsi.Domain.Interfaces;
using Konsi.Infrastructure.Messaging.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;

namespace Konsi.Infrastructure.Messaging.RabbitMQ;

public class RabbitMQService : IMessageQueueService
{
    private readonly RabbitMQSettings _settings;
    private readonly ICacheService _cacheService;
    private readonly ILogger<RabbitMQService> _logger;


    public RabbitMQService(IOptions<RabbitMQSettings> settings, ICacheService cacheService, ILogger<RabbitMQService> logger)
    {
        _settings = settings.Value;
        _cacheService = cacheService;
        _logger = logger;

    }

    public async Task PublishCpfAsync(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            throw new ArgumentException("O campo CPF não pode ser nulo", nameof(cpf));

        try
        {
            await _cacheService.GetCachedDataAsync(cpf);

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

            _logger.LogInformation($"CPF {cpf} publicado {_settings.QueueName} na fila com sucesso.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erroao publicar o {cpf} em {_settings.QueueName}.");
            throw;
        }
    }
}
using Konsi.Domain.Interfaces;
using Konsi.Infrastructure.Messaging.Configuration;
using Konsi.Infrastructure.Messaging.RabbitMQ;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using StackExchange.Redis;
using Xunit;

namespace Tests.Infrastructure.Tests.MessagingTests;

public class RabbitMQServiceTests
{
    private readonly Mock<IOptions<RabbitMQSettings>> _settingsMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly Mock<ILogger<RabbitMQService>> _loggerMock;
    private readonly RabbitMQService _rabbitMQService;
    private readonly Mock<IConnectionMultiplexer> _mockConnectionMultiplexer;


    public RabbitMQServiceTests()
    {
        _settingsMock = new Mock<IOptions<RabbitMQSettings>>();
        _cacheServiceMock = new Mock<ICacheService>();
        _loggerMock = new Mock<ILogger<RabbitMQService>>();
        _mockConnectionMultiplexer = new Mock<IConnectionMultiplexer>();


        _settingsMock.Setup(s => s.Value).Returns(new RabbitMQSettings
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest",
            QueueName = "cpf_queue"
        });

        _rabbitMQService = new RabbitMQService(_settingsMock.Object, _cacheServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task PublishCpfAsync_PublishesMessage()
    {
        // Arrange
        var cpf = "12345678900";
        _cacheServiceMock.Setup(x => x.GetCachedDataAsync(cpf)).ReturnsAsync(string.Empty);

        // Act
        await _rabbitMQService.PublishCpfAsync(cpf);

        // Assert

        _loggerMock.Verify(log => log.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => true),
            null,
            It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.AtLeastOnce);
    }
}

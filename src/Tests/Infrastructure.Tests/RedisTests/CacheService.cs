using Konsi.Domain.Interfaces;
using Konsi.Infrastructure.Redis.Data;
using Moq;
using StackExchange.Redis;
using System.Text.Json;
using Xunit;


namespace Tests.Infrastructure.Tests.RedisTests;
public class CacheServiceTests
{
    private readonly Mock<IConnectionMultiplexer> _mockConnectionMultiplexer;
    private readonly Mock<IDatabase> _mockDatabase;
    private readonly ICacheService _cacheService;

    public CacheServiceTests()
    {
        _mockConnectionMultiplexer = new Mock<IConnectionMultiplexer>();
        _mockDatabase = new Mock<IDatabase>();

        _mockConnectionMultiplexer
            .Setup(c => c.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(_mockDatabase.Object);

        _cacheService = new CacheService(_mockConnectionMultiplexer.Object);
    }

    [Fact]
    public async Task GetCachedDataAsync_ReturnsData()
    {
        // Arrange
        string key = "testKey";
        string expectedValue = "testData";
        _mockDatabase.Setup(db => db.StringGetAsync(key, It.IsAny<CommandFlags>()))
                     .ReturnsAsync(expectedValue);

        // Act
        string actualValue = await _cacheService.GetCachedDataAsync(key);

        // Assert
        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public async Task CacheDataAsync_ThrowsExceptionWhenSavingFails()
    {
        // Arrange
        string key = "testKey";
        object data = new { Name = "Test" };
        string jsonData = JsonSerializer.Serialize(data);

        _mockDatabase.Setup(db => db.StringSetAsync(key, jsonData, null, When.Always, CommandFlags.None))
                     .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _cacheService.CacheDataAsync(key, data));
        Assert.Equal("Falha ao salvar os dados no cache do Redis.", exception.Message);
    }
}
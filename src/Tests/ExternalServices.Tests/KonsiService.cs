using Konsi.API.ExternalServices.AppSettings;
using Konsi.API.ExternalServices.Services;
using Konsi.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Nest;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Tests.ExternalServices.Tests;
public class KonsiServiceTests
{
    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
    private readonly Mock<IOptions<KonsiSettings>> _mockOptions;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<IElasticsearchService> _mockElasticsearchService;
    private readonly Mock<ILogger<KonsiService>> _mockLogger;
    private readonly Mock<IElasticClient> _mockElasticClient;

    public KonsiServiceTests()
    {
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockOptions = new Mock<IOptions<KonsiSettings>>();
        _mockCacheService = new Mock<ICacheService>();
        _mockElasticsearchService = new Mock<IElasticsearchService>();
        _mockLogger = new Mock<ILogger<KonsiService>>();
        _mockElasticClient = new Mock<IElasticClient>();
    }

    [Fact]
    public async Task GetToken_ReturnsTokenOnSuccess()
    {
        // Arrange
        var settings = new KonsiSettings { ApiUrl = "https://api.example.com", Name = "user", Password = "pass" };
        _mockOptions.Setup(o => o.Value).Returns(settings);

        var responseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonContent.Create(new { data = new { token = "test-token" } })
        };

        var mockHttpMessageHandler = new MockHttpMessageHandler(responseMessage);
        var httpClient = new HttpClient(mockHttpMessageHandler)
        {
            BaseAddress = new Uri(settings.ApiUrl)
        };
        _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var konsiService = new KonsiService(_mockHttpClientFactory.Object, _mockOptions.Object, _mockCacheService.Object, _mockElasticsearchService.Object, _mockElasticClient.Object, _mockLogger.Object);

        // Act
        var token = await konsiService.GetToken();

        // Assert
        Assert.Equal("test-token", token);
    }

    [Fact]
    public async Task GetBenefitDataByCpf_ReturnsDataFromCacheIfExists()
    {
        // Arrange
        string cpf = "12345678901";
        var cachedData = "{\"cpf\":\"12345678901\",\"beneficios\":[{\"numero_beneficio\":\"123\",\"codigo_tipo_beneficio\":\"01\"}]}";
        _mockCacheService.Setup(x => x.GetCachedDataAsync(It.IsAny<string>())).ReturnsAsync(cachedData);

        var konsiService = new KonsiService(_mockHttpClientFactory.Object, _mockOptions.Object, _mockCacheService.Object, _mockElasticsearchService.Object, _mockElasticClient.Object, _mockLogger.Object);

        // Act
        var result = await konsiService.GetBenefitDataByCpf(cpf);

        // Assert
        Assert.NotNull(result);
    }
}





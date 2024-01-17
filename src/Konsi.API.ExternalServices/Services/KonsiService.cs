using Konsi.API.ExternalServices.AppSettings;
using Konsi.API.ExternalServices.Interfaces;
using Konsi.API.ExternalServices.Request;
using Konsi.API.ExternalServices.Response;
using Konsi.Domain.Interfaces;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;
using Konsi.Infrastructure.Redis.Data;
using Microsoft.Extensions.Logging;

namespace Konsi.API.ExternalServices.Services;

public class KonsiService : IKonsiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly KonsiSettings _settings;
    private readonly CacheService _cacheService;
    private readonly IElasticsearchService _elasticsearchService;
    private readonly ILogger<KonsiService> _logger;

    public KonsiService(IHttpClientFactory httpClientFactory, IOptions<KonsiSettings> settings, CacheService cacheService, IElasticsearchService elasticsearchService, ILogger<KonsiService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _settings = settings.Value;
        _cacheService = cacheService;
        _elasticsearchService = elasticsearchService;
        _logger = logger;
    }

    public async Task<string> GetToken()
    {
        _logger.LogInformation("Iniciando o processo de obter o token de acesso");

        var credentials = new TokenRequest
        {
            Username = _settings.Name,
            Password = _settings.Password
        };

        using var client = _httpClientFactory.CreateClient();
        var response = await client.PostAsJsonAsync(_settings.ApiUrl, credentials);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"Falha ao obter o token. Código de status: {response.StatusCode}");

            throw new Exception("Falha ao obter o token. Código de status: " + response.StatusCode);
        }

        var responseData = await response.Content.ReadFromJsonAsync<JsonElement>();
        return responseData.GetProperty("data").GetProperty("token").GetString();
    }

    public async Task<BenefitResponse> GetBenefitDataByCpf(string cpf)
    {
        _logger.LogInformation($"Iniciando a busca de benefícios para o CPF {cpf}");

        string cachedData = await _cacheService.GetCachedDataAsync(cpf);
        if (!string.IsNullOrEmpty(cachedData))
        {
            _logger.LogInformation($"Identificamos dados para o CPF {cpf} na base de dados.");

            return JsonSerializer.Deserialize<BenefitResponse>(cachedData);
        }

        _logger.LogInformation($"Não encontramos dados para o {cpf} em nossa base. Buscando via API externa");
        string token = await GetToken();
        var benefitResponse = await FetchBenefitByCpf(cpf, token);

        if (benefitResponse != null)
        {
            _logger.LogInformation($"Dados obtidos da API Konsi. Indexando no Elasticsearch o CPF {cpf}");

            await _elasticsearchService.IndexDataAsync("benefits", benefitResponse);
            await _cacheService.CacheDataAsync(cpf, benefitResponse);
        }

        return benefitResponse;
    }

    public async Task<BenefitResponse> FetchBenefitByCpf(string cpf, string token)
    {
        var url = $"{_settings.ApiUrlCpf}?cpf={cpf}";
        using var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

        _logger.LogInformation($"Realizando chamada à API Konsi para obter benefícios do CPF {cpf}");

        var response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"Falha na consulta de benefícios para o CPF {cpf}. Código de status: {response.StatusCode}");

            throw new Exception("Falha na consulta de benefícios. Código de status: " + response.StatusCode);
        }

        _logger.LogInformation($"Dados de benefícios obtidos com sucesso para o CPF {cpf}");

        return await response.Content.ReadFromJsonAsync<BenefitResponse>();
    }
}

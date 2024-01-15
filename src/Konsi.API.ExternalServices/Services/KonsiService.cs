using Konsi.API.ExternalServices.AppSettings;
using Konsi.API.ExternalServices.Interfaces;
using Konsi.API.ExternalServices.Request;
using Konsi.API.ExternalServices.Response;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;

namespace Konsi.API.ExternalServices.Services;

public class KonsiService : IKonsiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly KonsiSettings _settings;

    public KonsiService(IHttpClientFactory httpClientFactory, IOptions<KonsiSettings> settings)
    {
        _httpClientFactory = httpClientFactory;
        _settings = settings.Value;
    }

    public async Task<string> GetToken()
    {
        var credentials = new TokenRequest
        {
            Username = _settings.Name,
            Password = _settings.Password
        };

        using var client = _httpClientFactory.CreateClient();
        var response = await client.PostAsJsonAsync(_settings.ApiUrl, credentials);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Falha ao obter o token. Código de status: " + response.StatusCode);
        }

        var responseData = await response.Content.ReadFromJsonAsync<JsonElement>();
        return responseData.GetProperty("data").GetProperty("token").GetString();
    }

    public async Task<BenefitResponse> GetBenefitByCpf(string cpf, string token)
    {
        var url = $"{_settings.ApiUrlCpf}?cpf={cpf}";

        using var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

        var response = await client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Falha na consulta de benefícios. Código de status: " + response.StatusCode);
        }

        return await response.Content.ReadFromJsonAsync<BenefitResponse>();
    }
}
using Konsi.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Nest;

namespace Konsi.Infrastructure.Elasticsearch.Data;

public class ElasticsearchService : IElasticsearchService
{
    private readonly IElasticClient _elasticClient;
    private readonly ILogger<ElasticsearchService> _logger;


    public ElasticsearchService(IElasticClient elasticClient, ILogger<ElasticsearchService> logger)
    {
        _elasticClient = elasticClient;
        _logger = logger;
    }
    public async Task IndexDataAsync<T>(string index, T data) where T : class
    {
        if (data == null) throw new ArgumentNullException(nameof(data));

        try
        {
            var response = await _elasticClient.IndexAsync(data, idx => idx.Index(index));
            if (!response.IsValid)
            {
                _logger.LogError($"Erro ao indexar o documento {index}: {response.OriginalException.Message}");
            }
            else
            {
                _logger.LogInformation($"Documento indexado com sucesso {index}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Houve um erro inesperado ao indexar o ducumento {index}");
            throw;
        }
    }
}

namespace Konsi.Domain.Interfaces;

public interface IElasticsearchService
{
    Task IndexDataAsync<T>(string index, T data) where T : class;
}

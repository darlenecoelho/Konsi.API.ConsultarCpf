namespace Konsi.Domain.Interfaces;

public interface IMessageQueueService
{
    Task PublishCpfAsync(string cpf);

}

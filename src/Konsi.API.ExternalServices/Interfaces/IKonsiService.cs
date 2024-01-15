using Konsi.API.ExternalServices.Response;

namespace Konsi.API.ExternalServices.Interfaces;

public interface IKonsiService
{
    Task<string> GetToken();
    Task<BenefitResponse> GetBenefitByCpf(string cpf, string token);
}

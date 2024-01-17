using Konsi.API.ExternalServices.Response;

namespace Konsi.API.ExternalServices.Interfaces;

public interface IKonsiService
{
    Task<string> GetToken();
    Task<BenefitResponse> GetBenefitDataByCpf(string cpf);
    Task<BenefitResponse> FetchBenefitByCpf(string cpf, string token);
}

using Konsi.API.ExternalServices.Interfaces;
using Konsi.API.ExternalServices.Response;
using Konsi.Domain.Interfaces;
using Konsi.Infrastructure.Redis.Data;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Konsi.API.ConsultarCpf.Controllers;

[ApiController]
[Route("[controller]")]
public class KonsiController : ControllerBase
{
    private readonly IKonsiService _konsiService;
    private readonly IMessageQueueService _messageQueueService;
    private readonly CacheService _cacheService;

    public KonsiController(IKonsiService konsiService, IMessageQueueService messageQueueService, CacheService cacheService)
    {
        _konsiService = konsiService;
        _messageQueueService = messageQueueService;
        _cacheService = cacheService;
    }

    [HttpGet("consultar-beneficios/{cpf}")]
    public async Task<ActionResult> GetBenefitByCpf(string cpf)
    {
        try
        {
            await _messageQueueService.PublishCpfAsync(cpf);
            string cachedData = await _cacheService.GetCachedDataAsync(cpf);

            if (!string.IsNullOrEmpty(cachedData))
            {
                var benefit = JsonSerializer.Deserialize<BenefitResponse>(cachedData);
                return Ok(benefit);
            }

            var token = await _konsiService.GetToken();
            var benefitResponse = await _konsiService.GetBenefitByCpf(cpf, token);

            if (benefitResponse != null)
            {
                await _cacheService.CacheDataAsync(cpf, benefitResponse);
            }

            return Ok(benefitResponse);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }
}

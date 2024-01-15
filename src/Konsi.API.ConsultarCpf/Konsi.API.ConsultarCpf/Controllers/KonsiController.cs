using Konsi.API.ExternalServices.Interfaces;
using Konsi.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Konsi.API.ConsultarCpf.Controllers;

[ApiController]
[Route("[controller]")]
public class KonsiController : ControllerBase
{
    private readonly IKonsiService _konsiService;
    private readonly IMessageQueueService _messageQueueService;

    public KonsiController(IKonsiService konsiService, IMessageQueueService messageQueueService)
    {
        _konsiService = konsiService;
        _messageQueueService = messageQueueService;
    }

    [HttpGet("consultar-beneficios/{cpf}")]
    public async Task<ActionResult> ConsultarBeneficiosPorCPF(string cpf)
    {
        try
        {
            await _messageQueueService.PublishCpfAsync(cpf);
            var token = await _konsiService.GetToken();
            await _messageQueueService.PublishCpfAsync(cpf);

            var beneficios = await _konsiService.GetBenefitByCpf(cpf, token);

            return Ok(beneficios);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }
}

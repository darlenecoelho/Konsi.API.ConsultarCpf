using Konsi.API.ExternalServices.Interfaces;
using Konsi.API.ExternalServices.Response;
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

    /// <summary>
    /// Consulta os benefícios associados a um CPF via API externa.
    /// </summary>
    /// <param name="cpf"> O CPF para o qual os benefícios devem ser consultados.</param>
    /// <returns> Resultado da consulta.</returns>
    [HttpGet("consultar-beneficios/{cpf}")]
    public async Task<ActionResult> GetBenefitByCpf(string cpf)
    {
        try
        {
            await _messageQueueService.PublishCpfAsync(cpf);
            var benefitResponse = await _konsiService.GetBenefitDataByCpf(cpf);
            if (benefitResponse == null)
            {
                return NotFound("Benefícios para o CPF não encontrados.");
            }

            return Ok(benefitResponse);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }

    /// <summary>
    /// Consulta os benefícios associados a um CPF no Elasticsearch.
    /// </summary>
    /// <param name="cpf">O CPF para o qual os benefícios devem ser consultados no Elasticsearch.</param>
    /// <returns>Uma ação de resultado que representa o status da operação.</returns>
    [HttpGet("elasticsearch/{cpf}")]
    public async Task<ActionResult<BenefitResponse>> GetBenefitFromElasticsearchByCpf(string cpf)
    {
        try
        {
            var benefitResponse = await _konsiService.GetBenefitDataFromElasticsearchByCpf(cpf);
            if (benefitResponse == null)
            {
                return NotFound($"Nenhum benefício encontrado no Elasticsearch para o CPF {cpf}.");
            }

            return Ok(benefitResponse);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
        }
    }
}


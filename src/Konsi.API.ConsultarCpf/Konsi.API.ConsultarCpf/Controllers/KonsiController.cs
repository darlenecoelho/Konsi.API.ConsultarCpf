using Konsi.API.ExternalServices.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Konsi.API.ConsultarCpf.Controllers;

[ApiController]
[Route("[controller]")]
public class KonsiController : ControllerBase
{
    private readonly IKonsiService _konsiService;

    public KonsiController(IKonsiService konsiService)
    {
        _konsiService = konsiService;
    }

    [HttpGet("consultar-beneficios/{cpf}")]
    public async Task<ActionResult> ConsultarBeneficiosPorCPF(string cpf)
    {
        try
        {
            var token = await _konsiService.GetToken();
            var beneficios = await _konsiService.GetBenefitByCpf(cpf, token);

            return Ok(beneficios);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }
}

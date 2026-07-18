using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlataformaEducacao.Bff.Api.Services;

namespace PlataformaEducacao.Bff.Api.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("health")]
    public class HealthController(IHealthCheckService healthCheckService) : ControllerBase
    {
        private readonly IHealthCheckService _healthCheckService = healthCheckService;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var resposta = await _healthCheckService.VerificarSaude();

            return resposta.Sucesso
                ? Ok(resposta)
                : StatusCode(StatusCodes.Status503ServiceUnavailable, resposta);
        }
    }
}

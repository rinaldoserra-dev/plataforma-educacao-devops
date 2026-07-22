using Microsoft.Extensions.Options;
using PlataformaEducacao.Bff.Api.Extensions;
using PlataformaEducacao.Bff.Api.Models.Health;
using PlataformaEducacao.Core.Communication;

namespace PlataformaEducacao.Bff.Api.Services
{
    public interface IHealthCheckService
    {
        Task<ResponseResult> VerificarSaude();
    }

    public class HealthCheckService : Service, IHealthCheckService
    {
        private readonly HttpClient _httpClient;
        private readonly AppServicesSettings _settings;

        public HealthCheckService(HttpClient httpClient,
                                  IOptions<AppServicesSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }

        public async Task<ResponseResult> VerificarSaude()
        {
            var checks = new List<HealthStatusDTO>
            {
                await VerificarServico("Identidade", _settings.IdentidadeUrl),
                await VerificarServico("Gestao de Conteudo", _settings.GestaoConteudoUrl),
                await VerificarServico("Gestao de Alunos", _settings.GestaoAlunosUrl),
                await VerificarServico("Gestao Financeira", _settings.GestaoFinanceiraUrl)
            };

            var possuiFalha = checks.Any(check => check.Saudavel is false);

            return new ResponseResult
            {
                Status = possuiFalha ? StatusCodes.Status503ServiceUnavailable : StatusCodes.Status200OK,
                Sucesso = possuiFalha is false,
                Data = new
                {
                    Gateway = "PlataformaEducacao.Bff.Api",
                    TimestampUtc = DateTime.UtcNow,
                    Dependencias = checks
                },
                Erros = new ResponseErrorMessages
                {
                    Mensagens = possuiFalha ? ["Uma ou mais dependencias estao indisponiveis."] : []
                }
            };
        }

        private async Task<HealthStatusDTO> VerificarServico(string servico, string url)
        {
            try
            {
                using var response = await _httpClient.GetAsync($"{url.TrimEnd('/')}/swagger/index.html");

                return new HealthStatusDTO
                {
                    Servico = servico,
                    Url = url,
                    Saudavel = response.IsSuccessStatusCode,
                    Status = (int)response.StatusCode,
                    Mensagem = response.ReasonPhrase ?? "OK"
                };
            }
            catch (Exception ex)
            {
                return new HealthStatusDTO
                {
                    Servico = servico,
                    Url = url,
                    Saudavel = false,
                    Status = StatusCodes.Status503ServiceUnavailable,
                    Mensagem = ex.Message
                };
            }
        }
    }
}

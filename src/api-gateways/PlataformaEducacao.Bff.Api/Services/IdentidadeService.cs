using Microsoft.Extensions.Options;
using PlataformaEducacao.Bff.Api.Extensions;
using PlataformaEducacao.Bff.Api.Models.Request.Identidade;
using PlataformaEducacao.Core.Communication;

namespace PlataformaEducacao.Bff.Api.Services
{
    public interface IIdentidadeService
    {
        Task<ResponseResult> RegistrarAluno(RegistroAlunoRequest aluno);
        Task<ResponseResult> Login(LoginRequest login);
        Task<ResponseResult> RefreshToken(RefreshTokenRequest refreshToken);
    }

    public class IdentidadeService : Service, IIdentidadeService
    {
        private readonly HttpClient _httpClient;

        public IdentidadeService(HttpClient httpClient, IOptions<AppServicesSettings> settings)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(settings.Value.IdentidadeUrl);
        }

        public async Task<ResponseResult> Login(LoginRequest login)
        {
            var loginContent = ObterConteudo(login);

            var response = await _httpClient.PostAsync("/api/identidade/autenticar/", loginContent);

            return await DeserializarObjetoResponse(response);
        }

        public async Task<ResponseResult> RegistrarAluno(RegistroAlunoRequest aluno)
        {
            var alunoContent = ObterConteudo(aluno);

            var response = await _httpClient.PostAsync("/api/identidade/novo-aluno/", alunoContent);

            return await DeserializarObjetoResponse(response);

        }

        public async Task<ResponseResult> RefreshToken(RefreshTokenRequest refreshToken)
        {
            var refreshTokenContent = ObterConteudo(refreshToken);

            var response = await _httpClient.PostAsync("/api/identidade/refresh-token/", refreshTokenContent);

            return await DeserializarObjetoResponse(response);

        }
    }
}

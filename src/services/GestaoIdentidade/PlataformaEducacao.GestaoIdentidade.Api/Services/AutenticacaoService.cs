using Microsoft.Extensions.Options;
using PlataformaEducacao.GestaoIdentidade.Api.Data.Repository;
using PlataformaEducacao.GestaoIdentidade.Api.Models;
using PlataformaEducacao.WebApi.Core.Identidade;

namespace PlataformaEducacao.GestaoIdentidade.Api.Services
{
    public class AutenticacaoService : IAutenticacaoService
    {
        private readonly AppSettings _appSettings;
        private readonly IAutenticacaoRepository _autenticacaoRepository;

        public AutenticacaoService(IOptions<AppSettings> appSettings, IAutenticacaoRepository autenticacaoRepository)
        {
            _appSettings = appSettings.Value;
            _autenticacaoRepository = autenticacaoRepository;
        }

        public async Task<RefreshToken> GerarRefreshToken(string userName)
        {
            var refreshToken = new RefreshToken
            {
                UserName = userName,
                ExpirationDate = DateTime.UtcNow.AddHours(_appSettings.ExpiracaoRefrehToken),
            };

            await _autenticacaoRepository.AdicionarRefreshToken(refreshToken);

            return refreshToken;
        }

        public async Task<RefreshToken> ObterRefreshToken(Guid refreshToken)
        {
            return await _autenticacaoRepository.ObterRefreshToken(refreshToken);
        }
    }
}

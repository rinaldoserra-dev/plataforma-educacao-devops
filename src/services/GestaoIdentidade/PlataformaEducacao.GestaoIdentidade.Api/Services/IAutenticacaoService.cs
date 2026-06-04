using PlataformaEducacao.GestaoIdentidade.Api.Models;

namespace PlataformaEducacao.GestaoIdentidade.Api.Services
{
    public interface IAutenticacaoService
    {
        Task<RefreshToken> GerarRefreshToken(string userName);
        Task<RefreshToken> ObterRefreshToken(Guid refreshToken);
    }
}

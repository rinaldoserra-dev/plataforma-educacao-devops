using PlataformaEducacao.GestaoIdentidade.Api.Models;

namespace PlataformaEducacao.GestaoIdentidade.Api.Data.Repository
{
    public interface IAutenticacaoRepository
    {
        Task AdicionarRefreshToken(RefreshToken refreshToken);
        Task<RefreshToken?> ObterRefreshToken(Guid refreshToken);
    }
}

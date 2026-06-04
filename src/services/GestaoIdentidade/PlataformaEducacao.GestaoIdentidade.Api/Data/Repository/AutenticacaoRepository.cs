using Microsoft.EntityFrameworkCore;
using PlataformaEducacao.GestaoIdentidade.Api.Models;

namespace PlataformaEducacao.GestaoIdentidade.Api.Data.Repository
{
    public class AutenticacaoRepository : IAutenticacaoRepository
    {
        private readonly GestaoIdentidadeContext _context;

        public AutenticacaoRepository(GestaoIdentidadeContext context)
        {
            _context = context;
        }

        public async Task AdicionarRefreshToken(RefreshToken refreshToken)
        {
            _context.RefreshTokens.RemoveRange(_context.RefreshTokens.Where(u => u.UserName == refreshToken.UserName));
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken> ObterRefreshToken(Guid refreshToken)
        {
            var token = await _context.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(u => u.Token == refreshToken);

            return token != null && token.ExpirationDate.ToLocalTime() > DateTime.Now ? token : null;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PlataformaEducacao.GestaoFinanceira.Api.Data
{
    public sealed class PagamentosContextFactory : IDesignTimeDbContextFactory<PagamentosContext>
    {
        public PagamentosContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<PagamentosContext>()
                .UseSqlServer("Server=localhost,1433;Database=GestaoFinanceira;User Id=sa;Password=Plataforma@2026;Encrypt=True;TrustServerCertificate=False")
                .Options;

            return new PagamentosContext(options);
        }
    }
}

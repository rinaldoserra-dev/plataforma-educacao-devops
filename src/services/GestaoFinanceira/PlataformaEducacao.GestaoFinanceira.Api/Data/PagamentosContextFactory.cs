using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PlataformaEducacao.GestaoFinanceira.Api.Data;

public sealed class PagamentosContextFactory : IDesignTimeDbContextFactory<PagamentosContext>
{
    public PagamentosContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<PagamentosContext>()
            .UseSqlServer("Server=localhost,1433;Database=GestaoFinanceira;User Id=sa;Password=Plataforma@2026;TrustServerCertificate=True")
            .Options;

        return new PagamentosContext(options);
    }
}

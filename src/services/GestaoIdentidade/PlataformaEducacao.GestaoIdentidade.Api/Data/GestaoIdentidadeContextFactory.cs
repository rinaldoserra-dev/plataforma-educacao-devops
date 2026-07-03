using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PlataformaEducacao.GestaoIdentidade.Api.Data;

public sealed class GestaoIdentidadeContextFactory : IDesignTimeDbContextFactory<GestaoIdentidadeContext>
{
    public GestaoIdentidadeContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<GestaoIdentidadeContext>()
            .UseSqlServer("Server=localhost,1433;Database=GestaoIdentidade;User Id=sa;Password=Plataforma@2026;TrustServerCertificate=True")
            .Options;

        return new GestaoIdentidadeContext(options);
    }
}

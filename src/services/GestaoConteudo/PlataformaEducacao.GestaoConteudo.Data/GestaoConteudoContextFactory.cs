using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PlataformaEducacao.GestaoConteudo.Data;

public sealed class GestaoConteudoContextFactory : IDesignTimeDbContextFactory<GestaoConteudoContext>
{
    public GestaoConteudoContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<GestaoConteudoContext>()
            .UseSqlServer("Server=localhost,1433;Database=GestaoConteudo;User Id=sa;Password=Plataforma@2026;TrustServerCertificate=True")
            .Options;

        return new GestaoConteudoContext(options);
    }
}

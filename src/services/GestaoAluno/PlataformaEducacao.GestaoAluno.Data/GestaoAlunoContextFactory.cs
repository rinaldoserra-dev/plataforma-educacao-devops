using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using FluentValidation.Results;
using PlataformaEducacao.Core.Mediator;
using PlataformaEducacao.Core.Messages;

namespace PlataformaEducacao.GestaoAluno.Data;

public sealed class GestaoAlunoContextFactory : IDesignTimeDbContextFactory<GestaoAlunoContext>
{
    public GestaoAlunoContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<GestaoAlunoContext>()
            .UseSqlServer("Server=localhost,1433;Database=GestaoAluno;User Id=sa;Password=Plataforma@2026;TrustServerCertificate=True")
            .Options;

        return new GestaoAlunoContext(options, new DesignTimeMediatorHandler());
    }

    private sealed class DesignTimeMediatorHandler : IMediatorHandler
    {
        public Task PublishEvent<T>(T evento) where T : Event => Task.CompletedTask;

        public Task<ValidationResult> SendCommand<T>(T comando) where T : Command =>
            Task.FromResult(new ValidationResult());
    }
}

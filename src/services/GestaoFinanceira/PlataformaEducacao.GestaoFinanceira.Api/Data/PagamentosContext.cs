using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using PlataformaEducacao.Core.Data;
using PlataformaEducacao.Core.Messages;
using PlataformaEducacao.GestaoFinanceira.Business.Models;

namespace PlataformaEducacao.GestaoFinanceira.Api.Data
{
    public sealed class PagamentosContext : DbContext, IUnitOfWork
    {
        public PagamentosContext(DbContextOptions<PagamentosContext> options)
            : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            ChangeTracker.AutoDetectChangesEnabled = false;
        }

        public DbSet<Pagamento> Pagamentos { get; set; } = default!;

        public DbSet<Transacao> Transacoes { get; set; } = default!;

        public async Task<bool> Commit()
        {
            return await SaveChangesAsync() > 0;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<ValidationResult>();
            modelBuilder.Ignore<Evento>();

            foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(
                e => e.GetProperties().Where(p => p.ClrType == typeof(string))))
                property.SetColumnType("varchar(100)");

            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys())) relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PagamentosContext).Assembly);
        }
    }
}

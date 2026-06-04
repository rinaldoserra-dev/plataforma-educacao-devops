using Microsoft.EntityFrameworkCore;
using PlataformaEducacao.Core.Data;
using PlataformaEducacao.Core.Messages;
using PlataformaEducacao.GestaoConteudo.Domain;

namespace PlataformaEducacao.GestaoConteudo.Data
{
    public class GestaoConteudoContext(DbContextOptions<GestaoConteudoContext> options) : DbContext(options), IUnitOfWork
    {
        public DbSet<Curso> Cursos { get; set; }
        public DbSet<Aula> Aulas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetProperties().Where(p => p.ClrType == typeof(string))))
                property.SetColumnType("varchar(255)");

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(GestaoConteudoContext).Assembly);

            modelBuilder.Ignore<Event>();
            //modelBuilder.Ignore<ValidationResult>();

            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys())) relationship.DeleteBehavior = DeleteBehavior.ClientCascade;

            base.OnModelCreating(modelBuilder);
        }
        public async Task<bool> Commit()
        {
            var isSuccess = await base.SaveChangesAsync() > 0;

            return isSuccess;
        }
    }
}

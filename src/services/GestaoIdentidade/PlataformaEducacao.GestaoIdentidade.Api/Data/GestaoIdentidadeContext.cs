using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PlataformaEducacao.GestaoIdentidade.Api.Models;

namespace PlataformaEducacao.GestaoIdentidade.Api.Data
{
    public class GestaoIdentidadeContext(DbContextOptions<GestaoIdentidadeContext> options) : IdentityDbContext(options)
    {
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("RefreshTokens");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.UserName)
                    .IsRequired();

                entity.Property(x => x.Token)
                    .IsRequired();

                entity.Property(x => x.ExpirationDate)
                    .IsRequired();

                entity.HasIndex(x => x.Token)
                    .IsUnique();

                entity.HasIndex(x => x.UserName);
            });
        }
    }
}

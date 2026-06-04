using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlataformaEducacao.GestaoConteudo.Domain;

namespace PlataformaEducacao.GestaoConteudo.Data.Mappings
{
    public class AulaMapping : IEntityTypeConfiguration<Aula>
    {
        public void Configure(EntityTypeBuilder<Aula> builder)
        {
            builder.ToTable("Aulas");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Titulo)
                .IsRequired();

            builder.Property(a => a.Conteudo)
                .HasMaxLength(1000)
               .IsRequired();

            builder.HasOne(a => a.Curso)
                .WithMany(c => c.Aulas)
                .HasForeignKey(a => a.CursoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

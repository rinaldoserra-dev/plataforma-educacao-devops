using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlataformaEducacao.GestaoAluno.Domain;

namespace PlataformaEducacao.GestaoAluno.Data.Mappings
{
    public class ProgressoAulaMapping : IEntityTypeConfiguration<ProgressoAula>
    {
        public void Configure(EntityTypeBuilder<ProgressoAula> builder)
        {
            builder.ToTable("ProgressoAulas");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.AulaId)
                .IsRequired();

            builder.Property(p => p.DataConclusao)
                .IsRequired();

            builder.HasOne(p => p.Matricula)
                .WithMany(m => m.ProgressoAulas)
                .HasForeignKey(p => p.MatriculaId);
        }
    }
}

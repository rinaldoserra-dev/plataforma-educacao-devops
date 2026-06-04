using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlataformaEducacao.GestaoAluno.Domain;

namespace PlataformaEducacao.GestaoAluno.Data.Mappings
{
    public class CertificadoMapping : IEntityTypeConfiguration<Certificado>
    {
        public void Configure(EntityTypeBuilder<Certificado> builder)
        {
            builder.ToTable("Certificados");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.CodigoVerificacao)
                .IsRequired();

            builder.HasIndex(c => c.CodigoVerificacao)
                  .IsUnique();

            builder.HasOne(c => c.Matricula)
                .WithOne(m => m.Certificado)
                .HasForeignKey<Certificado>(c => c.MatriculaId)
                .IsRequired();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlataformaEducacao.GestaoConteudo.Domain;

namespace PlataformaEducacao.GestaoConteudo.Data.Mappings
{
    public class CursoMapping : IEntityTypeConfiguration<Curso>
    {
        public void Configure(EntityTypeBuilder<Curso> builder)
        {
            builder.ToTable("Cursos");

            builder.HasKey(c => c.Id);

            builder.Property(p => p.Nome)
                .IsRequired();

            builder.Property(p => p.Valor)
               .IsRequired();

            builder.Property(p => p.Disponivel)
               .IsRequired();

            builder.OwnsOne(c => c.ConteudoProgramatico, conteudoProgramatico =>
            {
                conteudoProgramatico.Property(s => s.Descricao)
                    .HasColumnName("Descricao")
                    .HasMaxLength(1000);

                conteudoProgramatico.Property(s => s.CargaHoraria)
                    .HasColumnName("CargaHoraria");
            });
        }
    }
}

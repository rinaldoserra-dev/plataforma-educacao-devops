using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlataformaEducacao.GestaoAluno.Domain;

namespace PlataformaEducacao.GestaoAluno.Data.Mappings
{
    public class MatriculaMapping : IEntityTypeConfiguration<Matricula>
    {
        public void Configure(EntityTypeBuilder<Matricula> builder)
        {
            builder.ToTable("Matriculas");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.NomeCurso)
                .IsRequired();

            builder.Property(m => m.DataMatricula)
                .IsRequired();

            builder.Property(m => m.SituacaoMatricula)
                .IsRequired();

            builder.Property(m => m.CursoId)
                .IsRequired();

            builder.HasOne(m => m.Aluno)
                .WithMany(a => a.Matriculas)
                .HasForeignKey(m => m.AlunoId);

            builder.OwnsOne(c => c.HistoricoAprendizado, historicoAprendizado =>
            {
                historicoAprendizado.Property(s => s.TotalAulasCurso)
                    .IsRequired()
                    .HasDefaultValue(0)
                    .HasColumnName("TotalAulasCurso");

                historicoAprendizado.Property(s => s.ProgressoGeralCurso)
                    .IsRequired()
                    .HasDefaultValue(0)
                    .HasColumnName("ProgressoGeralCurso");

                historicoAprendizado.Property(s => s.SituacaoCurso)
                    .IsRequired()
                    .HasDefaultValue(SituacaoCurso.NaoIniciado)
                    .HasColumnName("SituacaoCurso");

                historicoAprendizado.Property(s => s.DataConclusao)
                    .HasColumnName("DataConclusao");
            });
        }
    }
}

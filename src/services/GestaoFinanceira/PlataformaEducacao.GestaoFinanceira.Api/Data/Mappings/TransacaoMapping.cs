using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlataformaEducacao.GestaoFinanceira.Business.Models;

namespace PlataformaEducacao.GestaoFinanceira.Api.Data.Mappings
{
    public class TransacaoMapping : IEntityTypeConfiguration<Transacao>
    {
        public void Configure(EntityTypeBuilder<Transacao> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(x => x.CustoTransacao)
                .HasPrecision(18, 2);
            builder.Property(x => x.ValorTotal)
                .HasPrecision(18, 2);

            // 1 : N => Pagamento : Transacao
            builder.HasOne(c => c.Pagamento)
                .WithMany(c => c.Transacoes)
                .HasForeignKey(c => c.PagamentoId);

            builder.ToTable("Transacoes");
        }
    }
}

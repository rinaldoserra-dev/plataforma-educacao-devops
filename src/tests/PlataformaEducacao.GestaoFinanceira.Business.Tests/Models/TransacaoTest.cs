using PlataformaEducacao.GestaoFinanceira.Business.Models;

namespace PlataformaEducacao.GestaoFinanceira.Business.Tests.Models
{
    public class TransacaoTest
    {
        [Fact(DisplayName = "Transacao deve atribuir propriedades corretamente")]
        [Trait("Categoria", "Gestão Financeira - Business - Transacao")]
        public void Transacao_DeveAtribuirPropriedadesCorretamente()
        {
            // Arrange & Act
            var pagamentoId = Guid.NewGuid();
            var transacao = new Transacao
            {
                CodigoAutorizacao = "AUTH123",
                BandeiraCartao = "MasterCard",
                DataTransacao = DateTime.UtcNow,
                ValorTotal = 200m,
                CustoTransacao = 6m,
                Status = StatusTransacao.Pago,
                TID = "TID456",
                NSU = "NSU789",
                PagamentoId = pagamentoId
            };

            // Assert
            Assert.Equal("AUTH123", transacao.CodigoAutorizacao);
            Assert.Equal("MasterCard", transacao.BandeiraCartao);
            Assert.Equal(200m, transacao.ValorTotal);
            Assert.Equal(6m, transacao.CustoTransacao);
            Assert.Equal(StatusTransacao.Pago, transacao.Status);
            Assert.Equal("TID456", transacao.TID);
            Assert.Equal("NSU789", transacao.NSU);
            Assert.Equal(pagamentoId, transacao.PagamentoId);
        }

        [Fact(DisplayName = "StatusTransacao deve conter valores esperados")]
        [Trait("Categoria", "Gestão Financeira - Business - StatusTransacao")]
        public void StatusTransacao_ValoresEsperados()
        {
            Assert.Equal(1, (int)StatusTransacao.Autorizado);
            Assert.Equal(2, (int)StatusTransacao.Pago);
            Assert.Equal(3, (int)StatusTransacao.Negado);
            Assert.Equal(4, (int)StatusTransacao.Estornado);
            Assert.Equal(5, (int)StatusTransacao.Cancelado);
        }

        [Fact(DisplayName = "TipoPagamento deve conter valores esperados")]
        [Trait("Categoria", "Gestão Financeira - Business - TipoPagamento")]
        public void TipoPagamento_ValoresEsperados()
        {
            Assert.Equal(1, (int)TipoPagamento.CartaoCredito);
            Assert.Equal(2, (int)TipoPagamento.Boleto);
        }
    }
}

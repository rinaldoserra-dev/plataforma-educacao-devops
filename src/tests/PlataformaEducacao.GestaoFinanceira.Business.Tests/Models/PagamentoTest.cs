using PlataformaEducacao.GestaoFinanceira.Business.Models;

namespace PlataformaEducacao.GestaoFinanceira.Business.Tests.Models
{
    public class PagamentoTest
    {
        [Fact(DisplayName = "Novo Pagamento deve inicializar com lista vazia de transações")]
        [Trait("Categoria", "Gestão Financeira - Business - Pagamento")]
        public void NovoPagamento_DeveInicializarComListaVazia()
        {
            // Act
            var pagamento = new Pagamento();

            // Assert
            Assert.NotNull(pagamento.Transacoes);
            Assert.Empty(pagamento.Transacoes);
        }

        [Fact(DisplayName = "AdicionarTransacao deve adicionar à coleção")]
        [Trait("Categoria", "Gestão Financeira - Business - Pagamento")]
        public void AdicionarTransacao_DeveAdicionarAColecao()
        {
            // Arrange
            var pagamento = new Pagamento
            {
                AlunoId = Guid.NewGuid(),
                MatriculaId = Guid.NewGuid(),
                TipoPagamento = TipoPagamento.CartaoCredito,
                Valor = 100m,
                DadosCartao = new DadosCartao("Fulano", "4111111111111111", "12/2030", "123")
            };

            var transacao = new Transacao
            {
                CodigoAutorizacao = "ABC123",
                BandeiraCartao = "Visa",
                DataTransacao = DateTime.UtcNow,
                ValorTotal = 100m,
                CustoTransacao = 3m,
                Status = StatusTransacao.Autorizado,
                TID = "TID123",
                NSU = "NSU123"
            };

            // Act
            pagamento.AdicionarTransacao(transacao);

            // Assert
            Assert.Single(pagamento.Transacoes);
            Assert.Contains(transacao, pagamento.Transacoes);
        }

        [Fact(DisplayName = "Pagamento deve ter propriedades corretas")]
        [Trait("Categoria", "Gestão Financeira - Business - Pagamento")]
        public void Pagamento_DeveAtribuirPropriedadesCorretamente()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var matriculaId = Guid.NewGuid();

            // Act
            var pagamento = new Pagamento
            {
                AlunoId = alunoId,
                MatriculaId = matriculaId,
                TipoPagamento = TipoPagamento.CartaoCredito,
                Valor = 500m,
                DadosCartao = new DadosCartao("Fulano", "4111111111111111", "12/2030", "123")
            };

            // Assert
            Assert.Equal(alunoId, pagamento.AlunoId);
            Assert.Equal(matriculaId, pagamento.MatriculaId);
            Assert.Equal(TipoPagamento.CartaoCredito, pagamento.TipoPagamento);
            Assert.Equal(500m, pagamento.Valor);
            Assert.NotNull(pagamento.DadosCartao);
        }
    }
}

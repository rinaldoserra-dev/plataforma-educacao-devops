using Microsoft.Extensions.Options;
using PlataformaEducacao.GestaoFinanceira.Business.Facade;
using PlataformaEducacao.GestaoFinanceira.Business.Models;
using PlataformaEducacao.GestaoFinanceira.EduPag;

namespace PlataformaEducacao.GestaoFinanceira.Business.Tests.Facade
{
    public class PagamentoCartaoCreditoFacadeTest
    {
        [Fact(DisplayName = "ParaTransacao deve converter Transaction para Transacao corretamente")]
        [Trait("Categoria", "Gestão Financeira - Facade - PagamentoCartaoCreditoFacade")]
        public void ParaTransacao_DeveConverterCorretamente()
        {
            // Arrange
            var transaction = new Transaction(new EduPagService("key32caracteres_abcdefghijklm", "iv16caracteres__"))
            {
                AuthorizationCode = "AUTH123",
                CardBrand = "MasterCard",
                TransactionDate = new DateTime(2026, 1, 1),
                Cost = 3m,
                Amount = 100m,
                Status = TransactionStatus.Authorized,
                Tid = "TID456",
                Nsu = "NSU789"
            };

            // Act
            var transacao = PagamentoCartaoCreditoFacade.ParaTransacao(transaction);

            // Assert
            Assert.NotEqual(Guid.Empty, transacao.Id);
            Assert.Equal(StatusTransacao.Autorizado, transacao.Status);
            Assert.Equal(100m, transacao.ValorTotal);
            Assert.Equal("MasterCard", transacao.BandeiraCartao);
            Assert.Equal("AUTH123", transacao.CodigoAutorizacao);
            Assert.Equal(3m, transacao.CustoTransacao);
            Assert.Equal(new DateTime(2026, 1, 1), transacao.DataTransacao);
            Assert.Equal("NSU789", transacao.NSU);
            Assert.Equal("TID456", transacao.TID);
        }

        [Fact(DisplayName = "ParaTransaction deve converter Transacao para Transaction corretamente")]
        [Trait("Categoria", "Gestão Financeira - Facade - PagamentoCartaoCreditoFacade")]
        public void ParaTransaction_DeveConverterCorretamente()
        {
            // Arrange
            var svc = new EduPagService("key32caracteres_abcdefghijklm", "iv16caracteres__");
            var transacao = new Transacao
            {
                Status = StatusTransacao.Pago,
                ValorTotal = 200m,
                BandeiraCartao = "Visa",
                CodigoAutorizacao = "AUTH999",
                CustoTransacao = 6m,
                NSU = "NSU111",
                TID = "TID222"
            };

            // Act
            var transaction = PagamentoCartaoCreditoFacade.ParaTransaction(transacao, svc);

            // Assert
            Assert.Equal(TransactionStatus.Paid, transaction.Status);
            Assert.Equal(200m, transaction.Amount);
            Assert.Equal("Visa", transaction.CardBrand);
            Assert.Equal("AUTH999", transaction.AuthorizationCode);
            Assert.Equal(6m, transaction.Cost);
            Assert.Equal("NSU111", transaction.Nsu);
            Assert.Equal("TID222", transaction.Tid);
        }

        [Fact(DisplayName = "ParaTransacao com status Refused deve converter para Negado")]
        [Trait("Categoria", "Gestão Financeira - Facade - PagamentoCartaoCreditoFacade")]
        public void ParaTransacao_StatusRefused_DeveConverterParaNegado()
        {
            // Arrange
            var transaction = new Transaction(new EduPagService("key32caracteres_abcdefghijklm", "iv16caracteres__"))
            {
                AuthorizationCode = "",
                CardBrand = "",
                TransactionDate = DateTime.UtcNow,
                Cost = 0,
                Amount = 0,
                Status = TransactionStatus.Refused,
                Tid = "",
                Nsu = ""
            };

            // Act
            var transacao = PagamentoCartaoCreditoFacade.ParaTransacao(transaction);

            // Assert
            Assert.Equal(StatusTransacao.Negado, transacao.Status);
        }

        [Fact(DisplayName = "ParaTransacao com status Cancelled deve converter para Cancelado")]
        [Trait("Categoria", "Gestão Financeira - Facade - PagamentoCartaoCreditoFacade")]
        public void ParaTransacao_StatusCancelled_DeveConverterParaCancelado()
        {
            // Arrange
            var transaction = new Transaction(new EduPagService("key32caracteres_abcdefghijklm", "iv16caracteres__"))
            {
                AuthorizationCode = "",
                CardBrand = "Visa",
                TransactionDate = DateTime.UtcNow,
                Cost = 0,
                Amount = 100m,
                Status = TransactionStatus.Cancelled,
                Tid = "T",
                Nsu = "N"
            };

            // Act
            var transacao = PagamentoCartaoCreditoFacade.ParaTransacao(transaction);

            // Assert
            Assert.Equal(StatusTransacao.Cancelado, transacao.Status);
        }

        [Fact(DisplayName = "AutorizarPagamento deve retornar transacao")]
        [Trait("Categoria", "Gestão Financeira - Facade - PagamentoCartaoCreditoFacade")]
        public async Task AutorizarPagamento_DeveRetornarTransacao()
        {
            // Arrange
            var options = Options.Create(new PagamentoConfig
            {
                DefaultApiKey = "key32caracteres_abcdefghijklmnop",
                DefaultEncryptionKey = "iv16caracteres__"
            });
            var facade = new PagamentoCartaoCreditoFacade(options);
            var pagamento = new Pagamento
            {
                Id = Guid.NewGuid(),
                MatriculaId = Guid.NewGuid(),
                AlunoId = Guid.NewGuid(),
                TipoPagamento = TipoPagamento.CartaoCredito,
                Valor = 100m,
                DadosCartao = new DadosCartao("Fulano", "4111111111111111", "12/2030", "123")
            };

            // Act
            var result = await facade.AutorizarPagamento(pagamento);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Status == StatusTransacao.Autorizado || result.Status == StatusTransacao.Negado);
        }

        [Fact(DisplayName = "CapturarPagamento deve retornar transacao com status Pago")]
        [Trait("Categoria", "Gestão Financeira - Facade - PagamentoCartaoCreditoFacade")]
        public async Task CapturarPagamento_DeveRetornarTransacaoPaga()
        {
            // Arrange
            var options = Options.Create(new PagamentoConfig
            {
                DefaultApiKey = "key32caracteres_abcdefghijklmnop",
                DefaultEncryptionKey = "iv16caracteres__"
            });
            var facade = new PagamentoCartaoCreditoFacade(options);
            var transacao = new Transacao
            {
                Status = StatusTransacao.Autorizado,
                ValorTotal = 100m,
                BandeiraCartao = "Visa",
                CodigoAutorizacao = "AUTH",
                CustoTransacao = 3m,
                NSU = "NSU",
                TID = "TID"
            };

            // Act
            var result = await facade.CapturarPagamento(transacao);

            // Assert
            Assert.Equal(StatusTransacao.Pago, result.Status);
        }

        [Fact(DisplayName = "CancelarAutorizacao deve retornar transacao com status Cancelado")]
        [Trait("Categoria", "Gestão Financeira - Facade - PagamentoCartaoCreditoFacade")]
        public async Task CancelarAutorizacao_DeveRetornarTransacaoCancelada()
        {
            // Arrange
            var options = Options.Create(new PagamentoConfig
            {
                DefaultApiKey = "key32caracteres_abcdefghijklmnop",
                DefaultEncryptionKey = "iv16caracteres__"
            });
            var facade = new PagamentoCartaoCreditoFacade(options);
            var transacao = new Transacao
            {
                Status = StatusTransacao.Autorizado,
                ValorTotal = 100m,
                BandeiraCartao = "Visa",
                CodigoAutorizacao = "AUTH",
                CustoTransacao = 3m,
                NSU = "NSU",
                TID = "TID"
            };

            // Act
            var result = await facade.CancelarAutorizacao(transacao);

            // Assert
            Assert.Equal(StatusTransacao.Cancelado, result.Status);
        }
    }
}

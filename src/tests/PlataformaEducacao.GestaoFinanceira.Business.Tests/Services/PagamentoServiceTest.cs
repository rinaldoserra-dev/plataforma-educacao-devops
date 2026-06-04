using Moq;
using PlataformaEducacao.Core.DomainObjects;
using PlataformaEducacao.Core.Messages.Integration;
using PlataformaEducacao.GestaoFinanceira.Api.Services;
using PlataformaEducacao.GestaoFinanceira.Business.Facade;
using PlataformaEducacao.GestaoFinanceira.Business.Models;
using PlataformaEducacao.MessageBus;

namespace PlataformaEducacao.GestaoFinanceira.Business.Tests.Services
{
    public class PagamentoServiceTest
    {
        private readonly Mock<IPagamentoCartaoCreditoFacade> _facadeMock;
        private readonly Mock<IPagamentoRepository> _repositoryMock;
        private readonly Mock<IMessageBus> _busMock;
        private readonly PagamentoService _service;

        public PagamentoServiceTest()
        {
            _facadeMock = new Mock<IPagamentoCartaoCreditoFacade>();
            _repositoryMock = new Mock<IPagamentoRepository>();
            _busMock = new Mock<IMessageBus>();
            _service = new PagamentoService(_facadeMock.Object, _repositoryMock.Object, _busMock.Object);
        }

        [Fact(DisplayName = "AutorizarPagamento com transação autorizada deve retornar sucesso")]
        [Trait("Categoria", "Gestão Financeira - PagamentoService")]
        public async Task AutorizarPagamento_TransacaoAutorizada_DeveRetornarSucesso()
        {
            // Arrange
            var pagamento = CriarPagamento();
            var transacao = new Transacao
            {
                Status = StatusTransacao.Autorizado,
                CodigoAutorizacao = "AUTH",
                BandeiraCartao = "Visa",
                ValorTotal = 100m,
                CustoTransacao = 3m,
                TID = "TID",
                NSU = "NSU",
                DataTransacao = DateTime.UtcNow
            };

            _facadeMock.Setup(f => f.AutorizarPagamento(pagamento)).ReturnsAsync(transacao);
            _repositoryMock.Setup(r => r.UnitOfWork.Commit()).ReturnsAsync(true);

            // Act
            var resultado = await _service.AutorizarPagamento(pagamento, CancellationToken.None);

            // Assert
            Assert.True(resultado.ValidationResult.IsValid);
            _repositoryMock.Verify(r => r.AdicionarPagamento(pagamento), Times.Once);
        }

        [Fact(DisplayName = "AutorizarPagamento com transação recusada deve retornar erro")]
        [Trait("Categoria", "Gestão Financeira - PagamentoService")]
        public async Task AutorizarPagamento_TransacaoRecusada_DeveRetornarErro()
        {
            // Arrange
            var pagamento = CriarPagamento();
            var transacao = new Transacao
            {
                Status = StatusTransacao.Negado,
                CodigoAutorizacao = "",
                BandeiraCartao = "",
                ValorTotal = 0,
                CustoTransacao = 0,
                TID = "",
                NSU = "",
                DataTransacao = DateTime.UtcNow
            };

            _facadeMock.Setup(f => f.AutorizarPagamento(pagamento)).ReturnsAsync(transacao);

            // Act
            var resultado = await _service.AutorizarPagamento(pagamento, CancellationToken.None);

            // Assert
            Assert.False(resultado.ValidationResult.IsValid);
            Assert.Contains(resultado.ValidationResult.Errors, e => e.ErrorMessage.Contains("Pagamento recusado"));
        }

        [Fact(DisplayName = "AutorizarPagamento com falha ao persistir deve retornar erro")]
        [Trait("Categoria", "Gestão Financeira - PagamentoService")]
        public async Task AutorizarPagamento_FalhaAoPersistir_DeveRetornarErro()
        {
            // Arrange
            var pagamento = CriarPagamento();
            var transacao = new Transacao
            {
                Status = StatusTransacao.Autorizado,
                CodigoAutorizacao = "AUTH",
                BandeiraCartao = "Visa",
                ValorTotal = 100m,
                CustoTransacao = 3m,
                TID = "TID",
                NSU = "NSU",
                DataTransacao = DateTime.UtcNow
            };

            _facadeMock.Setup(f => f.AutorizarPagamento(pagamento)).ReturnsAsync(transacao);
            _repositoryMock.Setup(r => r.UnitOfWork.Commit()).ReturnsAsync(false);

            // Setup para CancelarPagamento
            var transacoes = new List<Transacao> { transacao };
            _repositoryMock.Setup(r => r.ObterTransacoesPorMatriculaId(pagamento.MatriculaId)).ReturnsAsync(transacoes);
            var transacaoCancelada = new Transacao { Status = StatusTransacao.Cancelado, BandeiraCartao = "Visa", ValorTotal = 100m, TID = "TID", NSU = "NSU", CodigoAutorizacao = "" };
            _facadeMock.Setup(f => f.CancelarAutorizacao(It.IsAny<Transacao>())).ReturnsAsync(transacaoCancelada);
            _repositoryMock.Setup(r => r.UnitOfWork.Commit()).ReturnsAsync(false);

            // Act
            var resultado = await _service.AutorizarPagamento(pagamento, CancellationToken.None);

            // Assert
            Assert.False(resultado.ValidationResult.IsValid);
        }

        [Theory(DisplayName = "ObterStatusPorMatricula com pagamento existente deve retornar status")]
        [Trait("Categoria", "Gestão Financeira - PagamentoService")]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ObterStatusPorMatricula_PagamentoExistente_DeveRetornarStatus(bool isAdmin)
        {
            // Arrange
            var matriculaId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();
            var pagamento = CriarPagamento();
            pagamento.MatriculaId = matriculaId;

            var transacao = new Transacao
            {
                Status = StatusTransacao.Pago,
                DataTransacao = DateTime.UtcNow,
                CodigoAutorizacao = "AUTH",
                BandeiraCartao = "Visa",
                ValorTotal = 100m,
                CustoTransacao = 3m,
                TID = "TID",
                NSU = "NSU"
            };
            pagamento.AdicionarTransacao(transacao);

            _repositoryMock.Setup(r => r.ObterPagamentoPorMatriculaId(matriculaId, usuarioId, isAdmin)).ReturnsAsync(pagamento);

            // Act
            var resultado = await _service.ObterStatusPorMatricula(matriculaId, usuarioId, isAdmin);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(matriculaId, resultado!.MatriculaId);
            Assert.Equal("Pago", resultado.Status);
        }

        [Theory(DisplayName = "ObterStatusPorMatricula sem pagamento deve retornar null")]
        [Trait("Categoria", "Gestão Financeira - PagamentoService")]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ObterStatusPorMatricula_SemPagamento_DeveRetornarNull(bool isAdmin)
        {
            // Arrange
            var matriculaId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.ObterPagamentoPorMatriculaId(matriculaId, usuarioId, isAdmin)).ReturnsAsync((Pagamento?)null);

            // Act
            var resultado = await _service.ObterStatusPorMatricula(matriculaId, usuarioId, isAdmin);

            // Assert
            Assert.Null(resultado);
        }

        [Fact(DisplayName = "CapturarPagamento com transação autorizada deve capturar")]
        [Trait("Categoria", "Gestão Financeira - PagamentoService")]
        public async Task CapturarPagamento_TransacaoAutorizada_DeveCapturar()
        {
            // Arrange
            var matriculaId = Guid.NewGuid();
            var transacaoAutorizada = new Transacao
            {
                Status = StatusTransacao.Autorizado,
                PagamentoId = Guid.NewGuid(),
                CodigoAutorizacao = "AUTH",
                BandeiraCartao = "Visa",
                ValorTotal = 100m,
                CustoTransacao = 3m,
                TID = "TID",
                NSU = "NSU"
            };
            var transacaoCapturada = new Transacao
            {
                Status = StatusTransacao.Pago,
                CodigoAutorizacao = "AUTH2",
                BandeiraCartao = "Visa",
                ValorTotal = 100m,
                CustoTransacao = 0,
                TID = "TID",
                NSU = "NSU"
            };

            _repositoryMock.Setup(r => r.ObterTransacoesPorMatriculaId(matriculaId))
                .ReturnsAsync(new List<Transacao> { transacaoAutorizada });
            _facadeMock.Setup(f => f.CapturarPagamento(transacaoAutorizada)).ReturnsAsync(transacaoCapturada);
            _repositoryMock.Setup(r => r.UnitOfWork.Commit()).ReturnsAsync(true);

            // Act
            var resultado = await _service.CapturarPagamento(matriculaId);

            // Assert
            Assert.True(resultado.ValidationResult.IsValid);
            _repositoryMock.Verify(r => r.AdicionarTransacao(transacaoCapturada), Times.Once);
        }

        [Fact(DisplayName = "CancelarPagamento com transação autorizada deve cancelar")]
        [Trait("Categoria", "Gestão Financeira - PagamentoService")]
        public async Task CancelarPagamento_TransacaoAutorizada_DeveCancelar()
        {
            // Arrange
            var matriculaId = Guid.NewGuid();
            var transacaoAutorizada = new Transacao
            {
                Status = StatusTransacao.Autorizado,
                PagamentoId = Guid.NewGuid(),
                CodigoAutorizacao = "AUTH",
                BandeiraCartao = "Visa",
                ValorTotal = 100m,
                CustoTransacao = 3m,
                TID = "TID",
                NSU = "NSU"
            };
            var transacaoCancelada = new Transacao
            {
                Status = StatusTransacao.Cancelado,
                CodigoAutorizacao = "",
                BandeiraCartao = "Visa",
                ValorTotal = 100m,
                CustoTransacao = 0,
                TID = "TID",
                NSU = "NSU"
            };

            _repositoryMock.Setup(r => r.ObterTransacoesPorMatriculaId(matriculaId))
                .ReturnsAsync(new List<Transacao> { transacaoAutorizada });
            _facadeMock.Setup(f => f.CancelarAutorizacao(transacaoAutorizada)).ReturnsAsync(transacaoCancelada);
            _repositoryMock.Setup(r => r.UnitOfWork.Commit()).ReturnsAsync(true);

            // Act
            var resultado = await _service.CancelarPagamento(matriculaId);

            // Assert
            Assert.True(resultado.ValidationResult.IsValid);
            _repositoryMock.Verify(r => r.AdicionarTransacao(transacaoCancelada), Times.Once);
        }

        [Fact(DisplayName = "AutorizarPagamento com exceção no PublishAsync deve retornar erro")]
        [Trait("Categoria", "Gestão Financeira - PagamentoService")]
        public async Task AutorizarPagamento_ExcecaoNoPublish_DeveRetornarErro()
        {
            // Arrange
            var pagamento = CriarPagamento();
            var transacao = new Transacao
            {
                Status = StatusTransacao.Autorizado,
                CodigoAutorizacao = "AUTH",
                BandeiraCartao = "Visa",
                ValorTotal = 100m,
                CustoTransacao = 3m,
                TID = "TID",
                NSU = "NSU",
                DataTransacao = DateTime.UtcNow
            };

            _facadeMock.Setup(f => f.AutorizarPagamento(pagamento)).ReturnsAsync(transacao);

            var commitCount = 0;
            _repositoryMock.Setup(r => r.UnitOfWork.Commit()).ReturnsAsync(() =>
            {
                commitCount++;
                return commitCount == 1;
            });

            _busMock.Setup(b => b.PublishAsync(It.IsAny<MatriculaPagamentoRealizadoIntegrationEvent>()))
                .ThrowsAsync(new Exception("Falha no bus"));

            _repositoryMock.Setup(r => r.ObterTransacoesPorMatriculaId(pagamento.MatriculaId))
                .ReturnsAsync(new List<Transacao> { transacao });
            _facadeMock.Setup(f => f.CancelarAutorizacao(It.IsAny<Transacao>()))
                .ReturnsAsync(new Transacao { Status = StatusTransacao.Cancelado, BandeiraCartao = "Visa", ValorTotal = 100m, TID = "TID", NSU = "NSU", CodigoAutorizacao = "" });

            // Act
            var resultado = await _service.AutorizarPagamento(pagamento, CancellationToken.None);

            // Assert
            Assert.False(resultado.ValidationResult.IsValid);
            Assert.Contains(resultado.ValidationResult.Errors, e => e.ErrorMessage.Contains("houve falha ao notificar"));
        }

        [Fact(DisplayName = "CapturarPagamento com status não pago deve retornar erro")]
        [Trait("Categoria", "Gestão Financeira - PagamentoService")]
        public async Task CapturarPagamento_StatusNaoPago_DeveRetornarErro()
        {
            // Arrange
            var matriculaId = Guid.NewGuid();
            var transacaoAutorizada = new Transacao
            {
                Status = StatusTransacao.Autorizado,
                PagamentoId = Guid.NewGuid(),
                CodigoAutorizacao = "AUTH",
                BandeiraCartao = "Visa",
                ValorTotal = 100m,
                CustoTransacao = 3m,
                TID = "TID",
                NSU = "NSU"
            };
            var transacaoCapturada = new Transacao
            {
                Status = StatusTransacao.Negado,
                CodigoAutorizacao = "",
                BandeiraCartao = "",
                ValorTotal = 0m,
                CustoTransacao = 0,
                TID = "",
                NSU = ""
            };

            _repositoryMock.Setup(r => r.ObterTransacoesPorMatriculaId(matriculaId))
                .ReturnsAsync(new List<Transacao> { transacaoAutorizada });
            _facadeMock.Setup(f => f.CapturarPagamento(transacaoAutorizada)).ReturnsAsync(transacaoCapturada);

            // Act
            var resultado = await _service.CapturarPagamento(matriculaId);

            // Assert
            Assert.False(resultado.ValidationResult.IsValid);
            Assert.Contains(resultado.ValidationResult.Errors, e => e.ErrorMessage.Contains("capturar o pagamento"));
        }

        [Fact(DisplayName = "CapturarPagamento com falha ao persistir deve retornar erro")]
        [Trait("Categoria", "Gestão Financeira - PagamentoService")]
        public async Task CapturarPagamento_FalhaAoPersistir_DeveRetornarErro()
        {
            // Arrange
            var matriculaId = Guid.NewGuid();
            var transacaoAutorizada = new Transacao
            {
                Status = StatusTransacao.Autorizado,
                PagamentoId = Guid.NewGuid(),
                CodigoAutorizacao = "AUTH",
                BandeiraCartao = "Visa",
                ValorTotal = 100m,
                CustoTransacao = 3m,
                TID = "TID",
                NSU = "NSU"
            };
            var transacaoCapturada = new Transacao
            {
                Status = StatusTransacao.Pago,
                CodigoAutorizacao = "AUTH2",
                BandeiraCartao = "Visa",
                ValorTotal = 100m,
                CustoTransacao = 0,
                TID = "TID",
                NSU = "NSU"
            };

            _repositoryMock.Setup(r => r.ObterTransacoesPorMatriculaId(matriculaId))
                .ReturnsAsync(new List<Transacao> { transacaoAutorizada });
            _facadeMock.Setup(f => f.CapturarPagamento(transacaoAutorizada)).ReturnsAsync(transacaoCapturada);
            _repositoryMock.Setup(r => r.UnitOfWork.Commit()).ReturnsAsync(false);

            // Act
            var resultado = await _service.CapturarPagamento(matriculaId);

            // Assert
            Assert.False(resultado.ValidationResult.IsValid);
            Assert.Contains(resultado.ValidationResult.Errors, e => e.ErrorMessage.Contains("persistir a captura"));
        }

        [Fact(DisplayName = "CancelarPagamento com status não cancelado deve retornar erro")]
        [Trait("Categoria", "Gestão Financeira - PagamentoService")]
        public async Task CancelarPagamento_StatusNaoCancelado_DeveRetornarErro()
        {
            // Arrange
            var matriculaId = Guid.NewGuid();
            var transacaoAutorizada = new Transacao
            {
                Status = StatusTransacao.Autorizado,
                PagamentoId = Guid.NewGuid(),
                CodigoAutorizacao = "AUTH",
                BandeiraCartao = "Visa",
                ValorTotal = 100m,
                CustoTransacao = 3m,
                TID = "TID",
                NSU = "NSU"
            };

            _repositoryMock.Setup(r => r.ObterTransacoesPorMatriculaId(matriculaId))
                .ReturnsAsync(new List<Transacao> { transacaoAutorizada });
            _facadeMock.Setup(f => f.CancelarAutorizacao(transacaoAutorizada))
                .ReturnsAsync(new Transacao { Status = StatusTransacao.Negado, CodigoAutorizacao = "", BandeiraCartao = "", ValorTotal = 0, CustoTransacao = 0, TID = "", NSU = "" });

            // Act
            var resultado = await _service.CancelarPagamento(matriculaId);

            // Assert
            Assert.False(resultado.ValidationResult.IsValid);
            Assert.Contains(resultado.ValidationResult.Errors, e => e.ErrorMessage.Contains("cancelar o pagamento"));
        }

        [Fact(DisplayName = "CancelarPagamento com falha ao persistir deve retornar erro")]
        [Trait("Categoria", "Gestão Financeira - PagamentoService")]
        public async Task CancelarPagamento_FalhaAoPersistir_DeveRetornarErro()
        {
            // Arrange
            var matriculaId = Guid.NewGuid();
            var transacaoAutorizada = new Transacao
            {
                Status = StatusTransacao.Autorizado,
                PagamentoId = Guid.NewGuid(),
                CodigoAutorizacao = "AUTH",
                BandeiraCartao = "Visa",
                ValorTotal = 100m,
                CustoTransacao = 3m,
                TID = "TID",
                NSU = "NSU"
            };

            _repositoryMock.Setup(r => r.ObterTransacoesPorMatriculaId(matriculaId))
                .ReturnsAsync(new List<Transacao> { transacaoAutorizada });
            _facadeMock.Setup(f => f.CancelarAutorizacao(transacaoAutorizada))
                .ReturnsAsync(new Transacao { Status = StatusTransacao.Cancelado, CodigoAutorizacao = "", BandeiraCartao = "Visa", ValorTotal = 100m, CustoTransacao = 0, TID = "TID", NSU = "NSU" });
            _repositoryMock.Setup(r => r.UnitOfWork.Commit()).ReturnsAsync(false);

            // Act
            var resultado = await _service.CancelarPagamento(matriculaId);

            // Assert
            Assert.False(resultado.ValidationResult.IsValid);
            Assert.Contains(resultado.ValidationResult.Errors, e => e.ErrorMessage.Contains("persistir o cancelamento"));
        }

        [Fact(DisplayName = "CapturarPagamento sem transação autorizada deve lançar DomainException")]
        [Trait("Categoria", "Gestão Financeira - PagamentoService")]
        public async Task CapturarPagamento_SemTransacaoAutorizada_DeveLancarDomainException()
        {
            // Arrange
            var matriculaId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.ObterTransacoesPorMatriculaId(matriculaId))
                .ReturnsAsync(new List<Transacao>());

            // Act & Assert
            await Assert.ThrowsAsync<DomainException>(() => _service.CapturarPagamento(matriculaId));
        }

        [Fact(DisplayName = "CancelarPagamento sem transação autorizada deve lançar DomainException")]
        [Trait("Categoria", "Gestão Financeira - PagamentoService")]
        public async Task CancelarPagamento_SemTransacaoAutorizada_DeveLancarDomainException()
        {
            // Arrange
            var matriculaId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.ObterTransacoesPorMatriculaId(matriculaId))
                .ReturnsAsync(new List<Transacao>());

            // Act & Assert
            await Assert.ThrowsAsync<DomainException>(() => _service.CancelarPagamento(matriculaId));
        }

        [Theory(DisplayName = "ObterStatusPorMatricula sem transações deve retornar Sem transações")]
        [Trait("Categoria", "Gestão Financeira - PagamentoService")]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ObterStatusPorMatricula_SemTransacoes_DeveRetornarSemTransacoes(bool isAdmin)
        {
            // Arrange
            var matriculaId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();
            var pagamento = new Pagamento
            {
                AlunoId = usuarioId,
                MatriculaId = matriculaId,
                TipoPagamento = TipoPagamento.CartaoCredito,
                Valor = 100m,
                DadosCartao = new DadosCartao("Fulano", "4111111111111111", "12/2030", "123")
            };

            _repositoryMock.Setup(r => r.ObterPagamentoPorMatriculaId(matriculaId, usuarioId, isAdmin))
                .ReturnsAsync(pagamento);

            // Act
            var resultado = await _service.ObterStatusPorMatricula(matriculaId, usuarioId, isAdmin);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Sem transações", resultado!.Status);
        }

        private static Pagamento CriarPagamento()
        {
            return new Pagamento
            {
                AlunoId = Guid.NewGuid(),
                MatriculaId = Guid.NewGuid(),
                TipoPagamento = TipoPagamento.CartaoCredito,
                Valor = 100m,
                DadosCartao = new DadosCartao("Fulano", "4111111111111111", "12/2030", "123")
            };
        }
    }
}

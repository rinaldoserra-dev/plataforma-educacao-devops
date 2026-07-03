using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PlataformaEducacao.GestaoFinanceira.Api.Data;
using PlataformaEducacao.GestaoFinanceira.Api.Data.Repository;
using PlataformaEducacao.GestaoFinanceira.Business.Models;

namespace PlataformaEducacao.GestaoFinanceira.Api.Tests.Data
{
    public class PagamentoRepositoryIntegrationTest : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly PagamentosContext _context;
        private readonly PagamentoRepository _repository;

        public PagamentoRepositoryIntegrationTest()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<PagamentosContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new PagamentosContext(options);
            _context.Database.EnsureCreated();

            _repository = new PagamentoRepository(_context);
        }

        [Fact(DisplayName = "AdicionarPagamento e Commit deve persistir")]
        [Trait("Categoria", "GestaoFinanceira.Api - Data - PagamentoRepository")]
        public async Task AdicionarPagamento_DevePeristir()
        {
            var pagamento = CriarPagamento();

            _repository.AdicionarPagamento(pagamento);
            _context.ChangeTracker.DetectChanges();
            var result = await _context.SaveChangesAsync() > 0;

            Assert.True(result);
        }

        [Fact(DisplayName = "ObterPagamentoPorMatriculaId deve retornar pagamento")]
        [Trait("Categoria", "GestaoFinanceira.Api - Data - PagamentoRepository")]
        public async Task ObterPagamentoPorMatriculaId_DeveRetornar()
        {
            var pagamento = CriarPagamento();
            _context.ChangeTracker.AutoDetectChangesEnabled = true;
            _context.Pagamentos.Add(pagamento);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.AutoDetectChangesEnabled = false;

            var result = await _repository.ObterPagamentoPorMatriculaId(pagamento.MatriculaId);

            Assert.NotNull(result);
            Assert.Equal(pagamento.MatriculaId, result.MatriculaId);
        }

        [Theory(DisplayName = "ObterPagamentoPorMatriculaId com usuarioId deve retornar pagamento")]
        [Trait("Categoria", "GestaoFinanceira.Api - Data - PagamentoRepository")]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ObterPagamentoPorMatriculaIdEUsuarioId_DeveRetornar(bool isAdmin)
        {
            var pagamento = CriarPagamento();
            _context.ChangeTracker.AutoDetectChangesEnabled = true;
            _context.Pagamentos.Add(pagamento);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.AutoDetectChangesEnabled = false;

            var result = await _repository.ObterPagamentoPorMatriculaId(pagamento.MatriculaId, pagamento.AlunoId, isAdmin);

            Assert.NotNull(result);
        }

        [Theory(DisplayName = "ObterPagamentoPorMatriculaId inexistente deve retornar null")]
        [Trait("Categoria", "GestaoFinanceira.Api - Data - PagamentoRepository")]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ObterPagamentoPorMatriculaId_Inexistente_DeveRetornarNull(bool isAdmin)
        {
            var result = await _repository.ObterPagamentoPorMatriculaId(Guid.NewGuid(), Guid.NewGuid(), isAdmin);

            Assert.Null(result);
        }

        [Fact(DisplayName = "AdicionarTransacao deve persistir")]
        [Trait("Categoria", "GestaoFinanceira.Api - Data - PagamentoRepository")]
        public async Task AdicionarTransacao_DevePersistir()
        {
            var pagamento = CriarPagamento();
            _context.ChangeTracker.AutoDetectChangesEnabled = true;
            _context.Pagamentos.Add(pagamento);
            await _context.SaveChangesAsync();

            var transacao = new Transacao
            {
                CodigoAutorizacao = "AUTH123",
                BandeiraCartao = "Visa",
                DataTransacao = DateTime.UtcNow,
                ValorTotal = 100m,
                CustoTransacao = 3m,
                Status = StatusTransacao.Autorizado,
                TID = "TID123",
                NSU = "NSU123",
                PagamentoId = pagamento.Id
            };

            _repository.AdicionarTransacao(transacao);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.AutoDetectChangesEnabled = false;

            var result = await _context.Transacoes.AsNoTracking().FirstOrDefaultAsync(t => t.TID == "TID123");
            Assert.NotNull(result);
        }

        [Fact(DisplayName = "ObterTransacoesPorMatriculaId deve retornar transações")]
        [Trait("Categoria", "GestaoFinanceira.Api - Data - PagamentoRepository")]
        public async Task ObterTransacoesPorMatriculaId_DeveRetornar()
        {
            var pagamento = CriarPagamento();
            var transacao = new Transacao
            {
                CodigoAutorizacao = "AUTH456",
                BandeiraCartao = "Master",
                DataTransacao = DateTime.UtcNow,
                ValorTotal = 200m,
                CustoTransacao = 6m,
                Status = StatusTransacao.Pago,
                TID = "TID456",
                NSU = "NSU456",
                PagamentoId = pagamento.Id
            };
            pagamento.AdicionarTransacao(transacao);

            _context.ChangeTracker.AutoDetectChangesEnabled = true;
            _context.Pagamentos.Add(pagamento);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.AutoDetectChangesEnabled = false;

            var result = await _repository.ObterTransacoesPorMatriculaId(pagamento.MatriculaId);

            Assert.NotEmpty(result);
        }

        [Fact(DisplayName = "Commit deve retornar true quando há alterações")]
        [Trait("Categoria", "GestaoFinanceira.Api - Data - PagamentosContext")]
        public async Task Commit_ComAlteracao_DeveRetornarTrue()
        {
            var pagamento = CriarPagamento();
            _context.ChangeTracker.AutoDetectChangesEnabled = true;
            _context.Pagamentos.Add(pagamento);

            var result = await _context.Commit();

            Assert.True(result);
        }

        [Fact(DisplayName = "Commit sem alterações deve retornar false")]
        [Trait("Categoria", "GestaoFinanceira.Api - Data - PagamentosContext")]
        public async Task Commit_SemAlteracao_DeveRetornarFalse()
        {
            _context.ChangeTracker.AutoDetectChangesEnabled = true;
            var result = await _context.Commit();

            Assert.False(result);
        }

        [Fact(DisplayName = "UnitOfWork deve retornar o contexto")]
        [Trait("Categoria", "GestaoFinanceira.Api - Data - PagamentoRepository")]
        public void UnitOfWork_DeveRetornarContexto()
        {
            Assert.NotNull(_repository.UnitOfWork);
        }

        [Fact(DisplayName = "Dispose não deve lançar exceção")]
        [Trait("Categoria", "GestaoFinanceira.Api - Data - PagamentoRepository")]
        public void Dispose_NaoDeveLancarExcecao()
        {
            var exception = Record.Exception(() => _repository.Dispose());
            Assert.Null(exception);
        }

        private static Pagamento CriarPagamento()
        {
            return new Pagamento
            {
                Id = Guid.NewGuid(),
                AlunoId = Guid.NewGuid(),
                MatriculaId = Guid.NewGuid(),
                TipoPagamento = TipoPagamento.CartaoCredito,
                Valor = 100m,
                DadosCartao = new DadosCartao("Fulano", "4111111111111111", "12/2030", "123")
            };
        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Dispose();
        }
    }
}

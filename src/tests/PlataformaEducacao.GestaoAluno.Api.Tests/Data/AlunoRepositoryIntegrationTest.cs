using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using PlataformaEducacao.Core.Mediator;
using PlataformaEducacao.GestaoAluno.Data;
using PlataformaEducacao.GestaoAluno.Data.Repository;
using PlataformaEducacao.GestaoAluno.Domain;

namespace PlataformaEducacao.GestaoAluno.Api.Tests.Data
{
    public class AlunoRepositoryIntegrationTest : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly GestaoAlunoContext _context;
        private readonly AlunoRepository _repository;
        private readonly Mock<IMediatorHandler> _mediatorMock;

        public AlunoRepositoryIntegrationTest()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _mediatorMock = new Mock<IMediatorHandler>();

            var options = new DbContextOptionsBuilder<GestaoAlunoContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new GestaoAlunoContext(options, _mediatorMock.Object);
            _context.Database.EnsureCreated();

            _repository = new AlunoRepository(_context);
        }

        [Fact(DisplayName = "Inserir aluno deve persistir")]
        [Trait("Categoria", "GestaoAluno - Data - AlunoRepository")]
        public async Task Inserir_DevePersistir()
        {
            var aluno = new Aluno(Guid.NewGuid(), "Teste", "teste@teste.com");

            await _repository.Inserir(aluno, CancellationToken.None);
            var result = await _context.SaveChangesAsync() > 0;

            Assert.True(result);
        }

        [Fact(DisplayName = "ObterComMatriculasPorId deve retornar aluno com matriculas")]
        [Trait("Categoria", "GestaoAluno - Data - AlunoRepository")]
        public async Task ObterComMatriculasPorId_DeveRetornar()
        {
            var aluno = CriarAlunoComMatricula();
            await SalvarAluno(aluno);

            var result = await _repository.ObterComMatriculasPorId(aluno.Id, CancellationToken.None);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Matriculas);
        }

        [Fact(DisplayName = "ObterComMatriculasPorId inexistente deve retornar null")]
        [Trait("Categoria", "GestaoAluno - Data - AlunoRepository")]
        public async Task ObterComMatriculasPorId_Inexistente_DeveRetornarNull()
        {
            var result = await _repository.ObterComMatriculasPorId(Guid.NewGuid(), CancellationToken.None);

            Assert.Null(result);
        }

        [Fact(DisplayName = "RealizarMatricula deve persistir matricula")]
        [Trait("Categoria", "GestaoAluno - Data - AlunoRepository")]
        public async Task RealizarMatricula_DevePersistir()
        {
            var aluno = new Aluno(Guid.NewGuid(), "Aluno", "aluno@teste.com");
            await SalvarAluno(aluno);

            var matricula = new Matricula(Guid.NewGuid(), "Curso X", 5, 100m);
            aluno.RealizarMatricula(matricula);

            await _repository.RealizarMatricula(matricula, CancellationToken.None);
            var result = await _context.SaveChangesAsync() > 0;

            Assert.True(result);
        }

        [Fact(DisplayName = "ObterMatriculaComAlunoPorId deve retornar matricula com aluno")]
        [Trait("Categoria", "GestaoAluno - Data - AlunoRepository")]
        public async Task ObterMatriculaComAlunoPorId_DeveRetornar()
        {
            var aluno = CriarAlunoComMatricula();
            await SalvarAluno(aluno);
            var matriculaId = aluno.Matriculas.First().Id;

            var result = await _repository.ObterMatriculaComAlunoPorId(matriculaId, CancellationToken.None);

            Assert.NotNull(result);
            Assert.NotNull(result.Aluno);
        }

        [Fact(DisplayName = "ObterMatriculaComProgressoAulasPorId deve retornar matricula")]
        [Trait("Categoria", "GestaoAluno - Data - AlunoRepository")]
        public async Task ObterMatriculaComProgressoAulasPorId_DeveRetornar()
        {
            var aluno = CriarAlunoComMatricula();
            await SalvarAluno(aluno);
            var matriculaId = aluno.Matriculas.First().Id;

            var result = await _repository.ObterMatriculaComProgressoAulasPorId(matriculaId, CancellationToken.None);

            Assert.NotNull(result);
        }

        [Fact(DisplayName = "ObterMatriculasAtivasPorAlunoId deve retornar matriculas ativas")]
        [Trait("Categoria", "GestaoAluno - Data - AlunoRepository")]
        public async Task ObterMatriculasAtivasPorAlunoId_DeveRetornar()
        {
            var aluno = CriarAlunoComMatriculaAtiva();
            await SalvarAluno(aluno);

            var result = await _repository.ObterMatriculasAtivasPorAlunoId(aluno.Id, CancellationToken.None);

            Assert.NotEmpty(result);
        }

        [Fact(DisplayName = "ListarMatriculasPendentesPagamentoPorAlunoId deve retornar pendentes")]
        [Trait("Categoria", "GestaoAluno - Data - AlunoRepository")]
        public async Task ListarMatriculasPendentesPagamentoPorAlunoId_DeveRetornar()
        {
            var aluno = CriarAlunoComMatricula();
            await SalvarAluno(aluno);

            var result = await _repository.ListarMatriculasPendentesPagamentoPorAlunoId(aluno.Id, CancellationToken.None);

            Assert.NotEmpty(result);
        }

        [Fact(DisplayName = "ObterAlunosMatriculadosPorCursoId deve retornar matriculados ativos")]
        [Trait("Categoria", "GestaoAluno - Data - AlunoRepository")]
        public async Task ObterAlunosMatriculadosPorCursoId_DeveRetornar()
        {
            var aluno = CriarAlunoComMatriculaAtiva();
            var cursoId = aluno.Matriculas.First().CursoId;
            await SalvarAluno(aluno);

            var result = await _repository.ObterAlunosMatriculadosPorCursoId(cursoId, CancellationToken.None);

            Assert.NotEmpty(result);
        }

        [Fact(DisplayName = "ObterAlunosPendentesPorCursoId deve retornar pendentes")]
        [Trait("Categoria", "GestaoAluno - Data - AlunoRepository")]
        public async Task ObterAlunosPendentesPorCursoId_DeveRetornar()
        {
            var aluno = CriarAlunoComMatricula();
            var cursoId = aluno.Matriculas.First().CursoId;
            await SalvarAluno(aluno);

            var result = await _repository.ObterAlunosPendentesPorCursoId(cursoId, CancellationToken.None);

            Assert.NotEmpty(result);
        }

        [Fact(DisplayName = "AtualizarProgressoAula deve persistir progresso")]
        [Trait("Categoria", "GestaoAluno - Data - AlunoRepository")]
        public async Task AtualizarProgressoAula_DevePersistir()
        {
            var aluno = CriarAlunoComMatriculaAtiva();
            await SalvarAluno(aluno);

            var matricula = await _context.Matriculas
                .Include(m => m.ProgressoAulas)
                .Include(m => m.HistoricoAprendizado)
                .FirstAsync();

            var progresso = new ProgressoAula(Guid.NewGuid());
            matricula.RegistrarAula(progresso);

            await _repository.AtualizarProgressoAula(progresso, CancellationToken.None);
            var result = await _context.SaveChangesAsync() > 0;

            Assert.True(result);
        }

        [Fact(DisplayName = "GerarCertificado deve persistir certificado")]
        [Trait("Categoria", "GestaoAluno - Data - AlunoRepository")]
        public async Task GerarCertificado_DevePersistir()
        {
            var aluno = CriarAlunoComMatriculaAtiva();
            await SalvarAluno(aluno);

            var matriculaId = aluno.Matriculas.First().Id;
            var certificado = new Certificado(matriculaId);

            await _repository.GerarCertificado(certificado, CancellationToken.None);
            var result = await _context.SaveChangesAsync() > 0;

            Assert.True(result);
        }

        [Fact(DisplayName = "ObterMatriculaComCertificadoPorId deve retornar matricula com certificado")]
        [Trait("Categoria", "GestaoAluno - Data - AlunoRepository")]
        public async Task ObterMatriculaComCertificadoPorId_DeveRetornar()
        {
            var aluno = CriarAlunoComMatriculaAtiva();
            await SalvarAluno(aluno);

            var matriculaId = aluno.Matriculas.First().Id;
            var certificado = new Certificado(matriculaId);
            _context.Certificados.Add(certificado);
            await _context.SaveChangesAsync();

            var result = await _repository.ObterMatriculaComCertificadoPorId(matriculaId, CancellationToken.None);

            Assert.NotNull(result);
            Assert.NotNull(result.Certificado);
        }

        [Fact(DisplayName = "ObterCertificadoPorCodigoVerificacao deve retornar")]
        [Trait("Categoria", "GestaoAluno - Data - AlunoRepository")]
        public async Task ObterCertificadoPorCodigoVerificacao_DeveRetornar()
        {
            var aluno = CriarAlunoComMatriculaAtiva();
            await SalvarAluno(aluno);

            var matriculaId = aluno.Matriculas.First().Id;
            var certificado = new Certificado(matriculaId);
            _context.Certificados.Add(certificado);
            await _context.SaveChangesAsync();

            var result = await _repository.ObterCertificadoPorCodigoVerificacao(certificado.CodigoVerificacao, CancellationToken.None);

            Assert.NotNull(result);
        }

        [Fact(DisplayName = "ObterCertificadoPorCertificadoId deve retornar")]
        [Trait("Categoria", "GestaoAluno - Data - AlunoRepository")]
        public async Task ObterCertificadoPorCertificadoId_DeveRetornar()
        {
            var aluno = CriarAlunoComMatriculaAtiva();
            await SalvarAluno(aluno);

            var matriculaId = aluno.Matriculas.First().Id;
            var certificado = new Certificado(matriculaId);
            _context.Certificados.Add(certificado);
            await _context.SaveChangesAsync();

            var result = await _repository.ObterCertificadoPorCertificadoId(certificado.Id, CancellationToken.None);

            Assert.NotNull(result);
        }

        [Fact(DisplayName = "AtualizarMatricula deve atualizar")]
        [Trait("Categoria", "GestaoAluno - Data - AlunoRepository")]
        public async Task AtualizarMatricula_DeveAtualizar()
        {
            var aluno = CriarAlunoComMatriculaAtiva();
            await SalvarAluno(aluno);

            var matricula = await _context.Matriculas.FirstAsync();
            await _repository.AtualizarMatricula(matricula, CancellationToken.None);

            Assert.Equal(EntityState.Modified, _context.Entry(matricula).State);
        }

        [Fact(DisplayName = "Commit deve retornar true quando há alterações")]
        [Trait("Categoria", "GestaoAluno - Data - GestaoAlunoContext")]
        public async Task Commit_ComAlteracao_DeveRetornarTrue()
        {
            var aluno = new Aluno(Guid.NewGuid(), "Commit Test", "commit@teste.com");
            _context.Alunos.Add(aluno);

            var result = await _context.Commit();

            Assert.True(result);
        }

        [Fact(DisplayName = "Commit sem alterações deve retornar false")]
        [Trait("Categoria", "GestaoAluno - Data - GestaoAlunoContext")]
        public async Task Commit_SemAlteracao_DeveRetornarFalse()
        {
            var result = await _context.Commit();

            Assert.False(result);
        }

        [Fact(DisplayName = "UnitOfWork deve retornar o contexto")]
        [Trait("Categoria", "GestaoAluno - Data - AlunoRepository")]
        public void UnitOfWork_DeveRetornarContexto()
        {
            Assert.NotNull(_repository.UnitOfWork);
        }

        [Fact(DisplayName = "Dispose não deve lançar exceção")]
        [Trait("Categoria", "GestaoAluno - Data - AlunoRepository")]
        public void Dispose_NaoDeveLancarExcecao()
        {
            var exception = Record.Exception(() => _repository.Dispose());
            Assert.Null(exception);
        }

        private Aluno CriarAlunoComMatricula()
        {
            var aluno = new Aluno(Guid.NewGuid(), "Aluno Teste", "aluno@teste.com");
            var matricula = new Matricula(Guid.NewGuid(), "Curso Teste", 5, 100m);
            aluno.RealizarMatricula(matricula);
            return aluno;
        }

        private Aluno CriarAlunoComMatriculaAtiva()
        {
            var aluno = CriarAlunoComMatricula();
            aluno.ConcluirPagamentoMatricula(aluno.Matriculas.First());
            return aluno;
        }

        private async Task SalvarAluno(Aluno aluno)
        {
            _context.Alunos.Add(aluno);
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Dispose();
        }
    }
}

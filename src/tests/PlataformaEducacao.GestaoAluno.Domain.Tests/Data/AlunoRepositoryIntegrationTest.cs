using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using PlataformaEducacao.Core.Mediator;
using PlataformaEducacao.Core.Messages;
using PlataformaEducacao.GestaoAluno.Data;
using PlataformaEducacao.GestaoAluno.Data.Repository;

namespace PlataformaEducacao.GestaoAluno.Domain.Tests.Data
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
            _mediatorMock.Setup(m => m.PublishEvent(It.IsAny<Event>())).Returns(Task.CompletedTask);

            var options = new DbContextOptionsBuilder<GestaoAlunoContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new GestaoAlunoContext(options, _mediatorMock.Object);
            _context.Database.EnsureCreated();

            _repository = new AlunoRepository(_context);
        }

        [Fact(DisplayName = "Inserir aluno e obter com matrículas deve persistir")]
        [Trait("Categoria", "GestaoAluno - Data - AlunoRepository")]
        public async Task Inserir_EObterComMatriculas_DevePersistir()
        {
            // Arrange
            var aluno = new Aluno(Guid.NewGuid(), "João", "joao@test.com");

            // Act
            await _repository.Inserir(aluno, CancellationToken.None);
            await _context.SaveChangesAsync();

            var result = await _repository.ObterComMatriculasPorId(aluno.Id, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("João", result!.Nome);
            Assert.Equal("joao@test.com", result.Email.Endereco);
            Assert.Empty(result.Matriculas);
        }

        [Fact(DisplayName = "RealizarMatricula deve persistir matrícula")]
        [Trait("Categoria", "GestaoAluno - Data - AlunoRepository")]
        public async Task RealizarMatricula_DevePersistir()
        {
            // Arrange
            var aluno = new Aluno(Guid.NewGuid(), "Maria", "maria@test.com");
            await _repository.Inserir(aluno, CancellationToken.None);
            await _context.SaveChangesAsync();

            var matricula = new Matricula(Guid.NewGuid(), "Curso C#", totalAulasCurso: 3, valor: 200m);
            aluno.RealizarMatricula(matricula);

            // Act
            await _repository.RealizarMatricula(matricula, CancellationToken.None);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _repository.ObterMatriculaComAlunoPorId(matricula.Id, CancellationToken.None);
            Assert.NotNull(result);
            Assert.Equal("Curso C#", result!.NomeCurso);
            Assert.Equal(aluno.Id, result.AlunoId);
        }

        [Fact(DisplayName = "ObterMatriculasAtivasPorAlunoId deve retornar apenas ativas")]
        [Trait("Categoria", "GestaoAluno - Data - AlunoRepository")]
        public async Task ObterMatriculasAtivas_DeveRetornarApenasAtivas()
        {
            // Arrange
            var aluno = CriarAlunoComMatriculaAtiva("Carlos", "carlos@test.com");
            await _repository.Inserir(aluno, CancellationToken.None);
            var matriculaAtiva = aluno.Matriculas.First();
            await _repository.RealizarMatricula(matriculaAtiva, CancellationToken.None);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ObterMatriculasAtivasPorAlunoId(aluno.Id, CancellationToken.None);

            // Assert
            Assert.Single(result);
            Assert.Equal(SituacaoMatricula.Ativa, result.First().SituacaoMatricula);
        }

        [Fact(DisplayName = "ListarMatriculasPendentesPagamentoPorAlunoId deve retornar pendentes")]
        [Trait("Categoria", "GestaoAluno - Data - AlunoRepository")]
        public async Task ListarMatriculasPendentes_DeveRetornarPendentes()
        {
            // Arrange
            var aluno = new Aluno(Guid.NewGuid(), "Ana", "ana@test.com");
            var matricula = new Matricula(Guid.NewGuid(), "Curso Docker", totalAulasCurso: 5, valor: 300m);
            aluno.RealizarMatricula(matricula);
            await _repository.Inserir(aluno, CancellationToken.None);
            await _repository.RealizarMatricula(matricula, CancellationToken.None);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ListarMatriculasPendentesPagamentoPorAlunoId(aluno.Id, CancellationToken.None);

            // Assert
            Assert.Single(result);
            Assert.Equal(SituacaoMatricula.PendentePagamento, result.First().SituacaoMatricula);
        }

        [Fact(DisplayName = "ObterAlunosMatriculadosPorCursoId deve retornar ativos")]
        [Trait("Categoria", "GestaoAluno - Data - AlunoRepository")]
        public async Task ObterAlunosMatriculadosPorCursoId_DeveRetornarAtivos()
        {
            // Arrange
            var cursoId = Guid.NewGuid();
            var aluno = Matricula.MatriculaFactory.CriarComPagamentoAprovado(
                cursoId, "Curso K8s", 2, 500m,
                new Aluno(Guid.NewGuid(), "Pedro", "pedro@test.com"));
            await _context.Alunos.AddAsync(aluno.Aluno);
            await _context.Matriculas.AddAsync(aluno);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ObterAlunosMatriculadosPorCursoId(cursoId, CancellationToken.None);

            // Assert
            Assert.Single(result);
        }

        [Fact(DisplayName = "ObterAlunosPendentesPorCursoId deve retornar não ativos")]
        [Trait("Categoria", "GestaoAluno - Data - AlunoRepository")]
        public async Task ObterAlunosPendentesPorCursoId_DeveRetornarNaoAtivos()
        {
            // Arrange
            var cursoId = Guid.NewGuid();
            var aluno = new Aluno(Guid.NewGuid(), "Luca", "luca@test.com");
            var matricula = new Matricula(cursoId, "Curso Go", totalAulasCurso: 4, valor: 400m);
            aluno.RealizarMatricula(matricula);
            await _context.Alunos.AddAsync(aluno);
            await _context.Matriculas.AddAsync(matricula);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ObterAlunosPendentesPorCursoId(cursoId, CancellationToken.None);

            // Assert
            Assert.Single(result);
        }

        [Fact(DisplayName = "ObterMatriculaComProgressoAulasPorId deve retornar matrícula com progresso")]
        [Trait("Categoria", "GestaoAluno - Data - AlunoRepository")]
        public async Task ObterMatriculaComProgressoAulas_DeveRetornarComProgresso()
        {
            // Arrange
            var aluno = CriarAlunoComMatriculaAtiva("Sara", "sara@test.com");
            await _repository.Inserir(aluno, CancellationToken.None);
            var matricula = aluno.Matriculas.First();
            await _repository.RealizarMatricula(matricula, CancellationToken.None);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ObterMatriculaComProgressoAulasPorId(matricula.Id, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result!.ProgressoAulas);
        }

        [Fact(DisplayName = "AtualizarMatricula deve persistir alteração")]
        [Trait("Categoria", "GestaoAluno - Data - AlunoRepository")]
        public async Task AtualizarMatricula_DevePersistirAlteracao()
        {
            // Arrange
            var aluno = new Aluno(Guid.NewGuid(), "Marcos", "marcos@test.com");
            var matricula = new Matricula(Guid.NewGuid(), "Curso Java", totalAulasCurso: 3, valor: 250m);
            aluno.RealizarMatricula(matricula);
            await _repository.Inserir(aluno, CancellationToken.None);
            await _repository.RealizarMatricula(matricula, CancellationToken.None);
            await _context.SaveChangesAsync();

            // Ativar matrícula
            aluno.ConcluirPagamentoMatricula(matricula);
            await _repository.AtualizarMatricula(matricula, CancellationToken.None);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ObterMatriculaComAlunoPorId(matricula.Id, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(SituacaoMatricula.Ativa, result!.SituacaoMatricula);
        }

        [Fact(DisplayName = "AtualizarProgressoAula deve persistir progresso")]
        [Trait("Categoria", "GestaoAluno - Data - AlunoRepository")]
        public async Task AtualizarProgressoAula_DevePersistir()
        {
            // Arrange
            var aluno = CriarAlunoComMatriculaAtiva("Lucia", "lucia@test.com");
            await _repository.Inserir(aluno, CancellationToken.None);
            var matricula = aluno.Matriculas.First();
            await _repository.RealizarMatricula(matricula, CancellationToken.None);
            await _context.SaveChangesAsync();

            var progresso = new ProgressoAula(Guid.NewGuid());
            progresso.AssociarMatricula(matricula.Id);

            // Act
            await _repository.AtualizarProgressoAula(progresso, CancellationToken.None);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _context.ProgressoAulas.FirstOrDefaultAsync(p => p.AulaId == progresso.AulaId);
            Assert.NotNull(result);
        }

        [Fact(DisplayName = "GerarCertificado deve persistir certificado")]
        [Trait("Categoria", "GestaoAluno - Data - AlunoRepository")]
        public async Task GerarCertificado_DevePersistir()
        {
            // Arrange
            var aluno = CriarAlunoComMatriculaAtiva("Rafael", "rafael@test.com");
            await _repository.Inserir(aluno, CancellationToken.None);
            var matricula = aluno.Matriculas.First();
            await _repository.RealizarMatricula(matricula, CancellationToken.None);
            await _context.SaveChangesAsync();

            var certificado = Certificado.CertificadoFactory.CriarCompleto(matricula, "CODIGO-123");

            // Act
            await _repository.GerarCertificado(certificado, CancellationToken.None);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _context.Certificados.FirstOrDefaultAsync(c => c.Id == certificado.Id);
            Assert.NotNull(result);
            Assert.Equal("CODIGO-123", result!.CodigoVerificacao);
        }

        [Fact(DisplayName = "ObterMatriculaComCertificadoPorId deve retornar com certificado")]
        [Trait("Categoria", "GestaoAluno - Data - AlunoRepository")]
        public async Task ObterMatriculaComCertificado_DeveRetornarComCertificado()
        {
            // Arrange
            var aluno = CriarAlunoComMatriculaAtiva("Julia", "julia@test.com");
            await _repository.Inserir(aluno, CancellationToken.None);
            var matricula = aluno.Matriculas.First();
            await _repository.RealizarMatricula(matricula, CancellationToken.None);
            await _context.SaveChangesAsync();

            var certificado = Certificado.CertificadoFactory.CriarCompleto(matricula, "CERT-456");
            await _repository.GerarCertificado(certificado, CancellationToken.None);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ObterMatriculaComCertificadoPorId(matricula.Id, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result!.Certificado);
            Assert.Equal("CERT-456", result.Certificado!.CodigoVerificacao);
        }

        [Fact(DisplayName = "ObterCertificadoPorCodigoVerificacao deve retornar matrícula")]
        [Trait("Categoria", "GestaoAluno - Data - AlunoRepository")]
        public async Task ObterCertificadoPorCodigoVerificacao_DeveRetornar()
        {
            // Arrange
            var aluno = CriarAlunoComMatriculaAtiva("Bruno", "bruno@test.com");
            await _repository.Inserir(aluno, CancellationToken.None);
            var matricula = aluno.Matriculas.First();
            await _repository.RealizarMatricula(matricula, CancellationToken.None);
            await _context.SaveChangesAsync();

            var certificado = Certificado.CertificadoFactory.CriarCompleto(matricula, "VERIF-789");
            await _repository.GerarCertificado(certificado, CancellationToken.None);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ObterCertificadoPorCodigoVerificacao("VERIF-789", CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result!.Certificado);
        }

        [Fact(DisplayName = "ObterCertificadoPorCertificadoId deve retornar certificado completo")]
        [Trait("Categoria", "GestaoAluno - Data - AlunoRepository")]
        public async Task ObterCertificadoPorCertificadoId_DeveRetornar()
        {
            // Arrange
            var aluno = CriarAlunoComMatriculaAtiva("Clara", "clara@test.com");
            await _repository.Inserir(aluno, CancellationToken.None);
            var matricula = aluno.Matriculas.First();
            await _repository.RealizarMatricula(matricula, CancellationToken.None);
            await _context.SaveChangesAsync();

            var certificado = Certificado.CertificadoFactory.CriarCompleto(matricula, "CERT-COMP");
            await _repository.GerarCertificado(certificado, CancellationToken.None);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ObterCertificadoPorCertificadoId(certificado.Id, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result!.Matricula);
            Assert.NotNull(result.Matricula.Aluno);
        }

        [Fact(DisplayName = "ObterComMatriculasPorId quando não existe deve retornar null")]
        [Trait("Categoria", "GestaoAluno - Data - AlunoRepository")]
        public async Task ObterComMatriculas_QuandoNaoExiste_DeveRetornarNull()
        {
            var result = await _repository.ObterComMatriculasPorId(Guid.NewGuid(), CancellationToken.None);
            Assert.Null(result);
        }

        [Fact(DisplayName = "Commit com alteração deve retornar true")]
        [Trait("Categoria", "GestaoAluno - Data - GestaoAlunoContext")]
        public async Task Commit_ComAlteracao_DeveRetornarTrue()
        {
            // Arrange
            var aluno = new Aluno(Guid.NewGuid(), "Teste Commit", "commit@test.com");
            await _repository.Inserir(aluno, CancellationToken.None);

            // Act
            var result = await _context.Commit();

            // Assert
            Assert.True(result);
            _mediatorMock.Verify(m => m.PublishEvent(It.IsAny<Event>()), Times.Never);
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

        private static Aluno CriarAlunoComMatriculaAtiva(string nome, string email)
        {
            var aluno = new Aluno(Guid.NewGuid(), nome, email);
            var matricula = new Matricula(Guid.NewGuid(), "Curso Teste", totalAulasCurso: 2, valor: 100m);
            aluno.RealizarMatricula(matricula);
            aluno.ConcluirPagamentoMatricula(matricula);
            return aluno;
        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Dispose();
        }
    }
}

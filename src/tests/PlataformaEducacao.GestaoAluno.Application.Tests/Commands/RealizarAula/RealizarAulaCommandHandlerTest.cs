using Moq;
using PlataformaEducacao.Core.Data;
using PlataformaEducacao.GestaoAluno.Application.Commands.RealizarAula;
using PlataformaEducacao.GestaoAluno.Domain;
using PlataformaEducacao.GestaoAluno.Domain.Repositories;

namespace PlataformaEducacao.GestaoAluno.Application.Tests.Commands.RealizarAula
{
    public class RealizarAulaCommandHandlerTest
    {
        readonly Aluno _aluno = new(alunoId: Guid.NewGuid(), "Fulano de Tal", "fulano@teste.com");

        [Fact(DisplayName = "RealizarAula quando comando inválido não chama repositório")]
        [Trait("Categoria", "Gestão Aluno - Application - Commands - RealizarAulaCommandHandler")]
        public async Task Handle_ComandoInvalido_NaoChamaRepositorio()
        {
            // Arrange
            var comandoInvalido = new RealizarAulaCommand(matriculaId: Guid.Empty, cursoId: Guid.Empty, aulaId: Guid.Empty);
            var repositorioMock = new Mock<IAlunoRepository>(MockBehavior.Strict);
            var handler = new RealizarAulaCommandHandler(repositorioMock.Object);

            // Act
            var resultado = await handler.Handle(comandoInvalido, CancellationToken.None);

            // Assert
            Assert.Same(comandoInvalido.ValidationResult, resultado);
            repositorioMock.Verify(r => r.ObterMatriculaComProgressoAulasPorId(
                It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact(DisplayName = "RealizarAula quando aula pertence a curso diferente adiciona erro")]
        [Trait("Categoria", "Gestão Aluno - Application - Commands - RealizarAulaCommandHandler")]
        public async Task Handle_AulaDeOutroCurso_AdicionaErro()
        {
            // Arrange
            var cursoMatricula = Guid.NewGuid();
            var cursoDiferente = Guid.NewGuid();

            var matricula = Matricula.MatriculaFactory.CriarComPagamentoAprovado(
                cursoId: cursoMatricula, "Curso X", totalAulasCurso: 1, valor: 100m, _aluno);

            // aulaId que pertence a outro curso (simulado)
            var aulaId = Guid.NewGuid();

            var comando = new RealizarAulaCommand(matricula.Id, cursoDiferente, aulaId);

            var repositorioMock = new Mock<IAlunoRepository>();
            repositorioMock.Setup(r => r.ObterMatriculaComProgressoAulasPorId(
                matricula.Id, It.IsAny<CancellationToken>())).ReturnsAsync(matricula);

            var handler = new RealizarAulaCommandHandler(repositorioMock.Object);

            // Act
            var resultado = await handler.Handle(comando, CancellationToken.None);

            // Assert
            Assert.False(resultado.IsValid);
            Assert.Contains(resultado.Errors, e => e.ErrorMessage == "Essa aula não faz parte do curso dessa matrícula.");
        }

        [Fact(DisplayName = "RealizarAula quando matrícula não existe adiciona erro")]
        [Trait("Categoria", "Gestão Aluno - Application - Commands - RealizarAulaCommandHandler")]
        public async Task Handle_MatriculaNaoExiste_AdicionaErro()
        {
            // Arrange
            var comando = new RealizarAulaCommand(matriculaId: Guid.NewGuid(), cursoId: Guid.NewGuid(), aulaId: Guid.NewGuid());
            var repositorioMock = new Mock<IAlunoRepository>();
            repositorioMock.Setup(r => r.ObterMatriculaComProgressoAulasPorId(
                comando.MatriculaId, It.IsAny<CancellationToken>())).ReturnsAsync((Matricula?)null);

            // Act
            var handler = new RealizarAulaCommandHandler(repositorioMock.Object);

            // Assert
            var resultado = await handler.Handle(comando, CancellationToken.None);

            Assert.False(resultado.IsValid);
            Assert.Contains(resultado.Errors, e => e.ErrorMessage == "Matrícula não encontrada.");
        }

        [Fact(DisplayName = "RealizarAula quando matrícula pendente adiciona erro")]
        [Trait("Categoria", "Gestão Aluno - Application - Commands - RealizarAulaCommandHandler")]
        public async Task Handle_MatriculaPendente_AdicionaErro()
        {
            // Arrange
            var matricula = new Matricula(
                cursoId: Guid.NewGuid(), "Curso X", totalAulasCurso: 1, valor: 100m);
            matricula.AssociarAluno(_aluno.Id);

            var comando = new RealizarAulaCommand(matricula.Id, matricula.CursoId, aulaId: Guid.NewGuid());

            var repositorioMock = new Mock<IAlunoRepository>();
            repositorioMock.Setup(r => r.ObterMatriculaComProgressoAulasPorId(
                matricula.Id, It.IsAny<CancellationToken>())).ReturnsAsync(matricula);

            var handler = new RealizarAulaCommandHandler(repositorioMock.Object);

            // Act
            var resultado = await handler.Handle(comando, CancellationToken.None);

            // Assert
            Assert.False(resultado.IsValid);
            Assert.Contains(resultado.Errors, e => e.ErrorMessage == "Matrícula pendente de pagamento.");
        }

        [Fact(DisplayName = "RealizarAula quando aula já realizada adiciona erro")]
        [Trait("Categoria", "Gestão Aluno - Application - Commands - RealizarAulaCommandHandler")]
        public async Task Handle_AulaJaRealizada_AdicionaErro()
        {
            // Arrange
            var matricula = Matricula.MatriculaFactory.CriarComPagamentoAprovado(
                cursoId: Guid.NewGuid(), "Curso X", totalAulasCurso: 1, valor: 100m, _aluno);
            var aulaId = Guid.NewGuid();
            matricula.RegistrarAula(new ProgressoAula(aulaId));

            var comando = new RealizarAulaCommand(matricula.Id, matricula.CursoId, aulaId);

            var repositorioMock = new Mock<IAlunoRepository>();
            repositorioMock.Setup(r => r.ObterMatriculaComProgressoAulasPorId(
                matricula.Id, It.IsAny<CancellationToken>())).ReturnsAsync(matricula);

            var handler = new RealizarAulaCommandHandler(repositorioMock.Object);

            // Act
            var resultado = await handler.Handle(comando, CancellationToken.None);

            // Assert
            Assert.False(resultado.IsValid);
            Assert.Contains(resultado.Errors, e => e.ErrorMessage == "Aula já realizada.");
        }

        [Theory(DisplayName = "RealizarAula quando válido persiste alterações e chama repositório")]
        [Trait("Categoria", "Gestão Aluno - Application - Commands - RealizarAulaCommandHandler")]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Handle_Valido_PersisteConformeCommit(bool resultadoCommit)
        {
            // Arrange
            var matricula = Matricula.MatriculaFactory.CriarComPagamentoAprovado(
                cursoId: Guid.NewGuid(), "Curso X", totalAulasCurso: 1, valor: 100m, _aluno);

            var comando = new RealizarAulaCommand(matricula.Id, matricula.CursoId, aulaId: Guid.NewGuid());

            var uowMock = new Mock<IUnitOfWork>();
            uowMock.Setup(u => u.Commit()).ReturnsAsync(resultadoCommit);

            var repositorioMock = new Mock<IAlunoRepository>();
            repositorioMock.Setup(r => r.ObterMatriculaComProgressoAulasPorId(
                matricula.Id, It.IsAny<CancellationToken>())).ReturnsAsync(matricula);
            repositorioMock.SetupGet(r => r.UnitOfWork).Returns(uowMock.Object);

            ProgressoAula? progressoCapturado = null;
            repositorioMock.Setup(r => r.AtualizarProgressoAula(
                It.IsAny<ProgressoAula>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask)
                    .Callback<ProgressoAula, CancellationToken>((p, ct) => progressoCapturado = p);

            Matricula? matriculaCapturada = null;
            repositorioMock.Setup(r => r.AtualizarMatricula(
                It.IsAny<Matricula>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask)
                    .Callback<Matricula, CancellationToken>((m, ct) => matriculaCapturada = m);

            var handler = new RealizarAulaCommandHandler(repositorioMock.Object);

            // Act
            var resultado = await handler.Handle(comando, CancellationToken.None);

            // Assert
            Assert.Equal(resultadoCommit, resultado.IsValid);
            if (!resultadoCommit)
                Assert.Contains(resultado.Errors, e => e.ErrorMessage == "Houve um erro ao persistir os dados");

            repositorioMock.Verify(r => r.AtualizarProgressoAula(
                It.IsAny<ProgressoAula>(), It.IsAny<CancellationToken>()), Times.Once);
            repositorioMock.Verify(r => r.AtualizarMatricula(
                It.IsAny<Matricula>(), It.IsAny<CancellationToken>()), Times.Once);
            repositorioMock.VerifyGet(r => r.UnitOfWork, Times.AtLeastOnce);
            uowMock.Verify(u => u.Commit(), Times.Once);

            Assert.NotNull(progressoCapturado);
            Assert.Equal(comando.AulaId, progressoCapturado.AulaId);
            Assert.NotNull(matriculaCapturada);
            Assert.Equal(matricula.Id, matriculaCapturada.Id);
        }
    }
}
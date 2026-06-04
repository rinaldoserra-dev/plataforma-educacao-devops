using Moq;
using PlataformaEducacao.Core.Data;
using PlataformaEducacao.GestaoAluno.Application.Commands.FinalizarCurso;
using PlataformaEducacao.GestaoAluno.Domain;
using PlataformaEducacao.GestaoAluno.Domain.Repositories;

namespace PlataformaEducacao.GestaoAluno.Application.Tests.Commands.FinalizarCurso
{
    public class FinalizarCursoCommandHandlerTest
    {
        readonly Aluno _aluno = new(Guid.NewGuid(), "Nome do Aluno", "email@teste.com");

        [Fact(DisplayName = "FinalizarCursoCommand quando comando inválido retorna validação e não chama repositório")]
        [Trait("Categoria", "Gestão Aluno - Application - Commands - FinalizarCursoCommandHandler")]
        public async Task Handle_ComandoInvalido_RetornaValidacaoENaoChamaRepositorio()
        {
            // Arrange
            var comandoInvalido = new FinalizarCursoCommand(Guid.Empty);
            var repositorioMock = new Mock<IAlunoRepository>(MockBehavior.Strict);
            var handler = new FinalizarCursoCommandHandler(repositorioMock.Object);

            // Act
            var resultado = await handler.Handle(comandoInvalido, CancellationToken.None);

            // Assert
            Assert.Same(comandoInvalido.ValidationResult, resultado);
            repositorioMock.Verify(r => r.ObterMatriculaComProgressoAulasPorId(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact(DisplayName = "FinalizarCursoCommand quando matrícula não encontrada adiciona erro")]
        [Trait("Categoria", "Gestão Aluno - Application - Commands - FinalizarCursoCommandHandler")]
        public async Task Handle_MatriculaNaoEncontrada_AdicionaErro()
        {
            // Arrange
            var matriculaId = Guid.NewGuid();
            var comando = new FinalizarCursoCommand(matriculaId);
            var repositorioMock = new Mock<IAlunoRepository>();
            repositorioMock.Setup(r => r.ObterMatriculaComProgressoAulasPorId(matriculaId, It.IsAny<CancellationToken>())).ReturnsAsync((Matricula?)null);

            var handler = new FinalizarCursoCommandHandler(repositorioMock.Object);

            // Act
            var resultado = await handler.Handle(comando, CancellationToken.None);

            // Assert
            Assert.False(resultado.IsValid);
            Assert.Contains(resultado.Errors, e => e.ErrorMessage == $"Matrícula {matriculaId} não encontrada.");
        }

        [Fact(DisplayName = "FinalizarCursoCommand quando matrícula pendente de pagamento adiciona erro")]
        [Trait("Categoria", "Gestão Aluno - Application - Commands - FinalizarCursoCommandHandler")]
        public async Task Handle_MatriculaPendentePagamento_AdicionaErro()
        {
            // Arrange
            var matricula = new Matricula(Guid.NewGuid(), "Curso", totalAulasCurso: 10, valor: 100m);
            matricula.AssociarAluno(_aluno.Id);

            var comando = new FinalizarCursoCommand(matricula.Id);

            var repositorioMock = new Mock<IAlunoRepository>();
            repositorioMock.Setup(r => r.ObterMatriculaComProgressoAulasPorId(matricula.Id, It.IsAny<CancellationToken>())).ReturnsAsync(matricula);

            var handler = new FinalizarCursoCommandHandler(repositorioMock.Object);

            // Act
            var resultado = await handler.Handle(comando, CancellationToken.None);

            // Assert
            Assert.False(resultado.IsValid);
            Assert.Contains(resultado.Errors, e => e.ErrorMessage == $"Matrícula {matricula.Id} pendente de pagamento.");
        }

        [Fact(DisplayName = "FinalizarCursoCommand quando existem aulas pendentes adiciona erro")]
        [Trait("Categoria", "Gestão Aluno - Application - Commands - FinalizarCursoCommandHandler")]
        public async Task Handle_AulasPendentes_AdicionaErro()
        {
            // Arrange
            var matricula = Matricula.MatriculaFactory.CriarComPagamentoAprovado(Guid.NewGuid(), "Nome do Curso", totalAulasCurso: 2, valor: 100m, _aluno);

            var comando = new FinalizarCursoCommand(matricula.Id);

            var repositorioMock = new Mock<IAlunoRepository>();
            repositorioMock.Setup(r => r.ObterMatriculaComProgressoAulasPorId(matricula.Id, It.IsAny<CancellationToken>())).ReturnsAsync(matricula);

            var handler = new FinalizarCursoCommandHandler(repositorioMock.Object);

            // Act
            var resultado = await handler.Handle(comando, CancellationToken.None);

            // Assert
            Assert.False(resultado.IsValid);
            Assert.Contains(resultado.Errors, e => e.ErrorMessage == "Existem aulas pendentes de visualização.");
        }

        [Fact(DisplayName = "FinalizarCursoCommand quando curso já finalizado adiciona erro")]
        [Trait("Categoria", "Gestão Aluno - Application - Commands - FinalizarCursoCommandHandler")]
        public async Task Handle_CursoJaFinalizado_AdicionaErro()
        {
            // Arrange
            var matricula = Matricula.MatriculaFactory.CriarComCursoFinalizado(Guid.NewGuid(), "Curso", totalAulasCurso: 1, valor: 100m, _aluno);

            var comando = new FinalizarCursoCommand(matricula.Id);

            var repositorioMock = new Mock<IAlunoRepository>();
            repositorioMock.Setup(r => r.ObterMatriculaComProgressoAulasPorId(matricula.Id, It.IsAny<CancellationToken>())).ReturnsAsync(matricula);

            var handler = new FinalizarCursoCommandHandler(repositorioMock.Object);

            // Act
            var resultado = await handler.Handle(comando, CancellationToken.None);

            // Assert
            Assert.False(resultado.IsValid);
            Assert.Contains(resultado.Errors, e => e.ErrorMessage == "Curso já finalizado.");
        }

        [Theory(DisplayName = "FinalizarCursoCommand quando válido persiste conforme Commit do UnitOfWork")]
        [Trait("Categoria", "Gestão Aluno - Application - Commands - FinalizarCursoCommandHandler")]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Handle_Valido_PersisteConformeCommit(bool resultadoCommit)
        {
            // Arrange
            var matricula = Matricula.MatriculaFactory.CriarComPagamentoAprovado(Guid.NewGuid(), "Nome do Curso", totalAulasCurso: 1, valor: 100m, _aluno);
            matricula.RegistrarAula(new ProgressoAula(Guid.NewGuid()));

            var comando = new FinalizarCursoCommand(matricula.Id);

            var uowMock = new Mock<IUnitOfWork>();
            uowMock.Setup(u => u.Commit()).ReturnsAsync(resultadoCommit);

            var repoMock = new Mock<IAlunoRepository>();
            repoMock.Setup(r => r.ObterMatriculaComProgressoAulasPorId(matricula.Id, It.IsAny<CancellationToken>())).ReturnsAsync(matricula);
            repoMock.SetupGet(r => r.UnitOfWork).Returns(uowMock.Object);

            var handler = new FinalizarCursoCommandHandler(repoMock.Object);

            // Act
            var result = await handler.Handle(comando, CancellationToken.None);

            // Assert
            Assert.Equal(resultadoCommit, result.IsValid);
            if (!resultadoCommit)
                Assert.Contains(result.Errors, e => e.ErrorMessage == "Houve um erro ao persistir os dados");

            repoMock.VerifyGet(r => r.UnitOfWork, Times.AtLeastOnce);
            uowMock.Verify(u => u.Commit(), Times.Once);
        }
    }
}
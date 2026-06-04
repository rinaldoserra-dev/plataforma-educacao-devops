using Moq;
using PlataformaEducacao.Core.Data;
using PlataformaEducacao.GestaoAluno.Application.Commands.MatricularAlunoCurso;
using PlataformaEducacao.GestaoAluno.Domain;
using PlataformaEducacao.GestaoAluno.Domain.Repositories;

namespace PlataformaEducacao.GestaoAluno.Application.Tests.Commands.MatricularAlunoCurso
{
    public class MatricularAlunoCursoCommandHandlerTest
    {
        readonly Aluno _aluno = new(alunoId: Guid.NewGuid(), "Fulano de Tal", "fulano@teste.com");

        [Fact(DisplayName = "MatricularAlunoCurso quando comando inválido retorna validação e não chama repositório")]
        [Trait("Categoria", "Gestão Aluno - Application - Commands - MatricularAlunoCursoHandler")]
        public async Task Handle_ComandoInvalido_RetornaValidacaoENaoChamaRepositorio()
        {
            // Arrange
            var comandoInvalido = new MatricularAlunoCursoCommand(
                cursoId: Guid.Empty, alunoId: Guid.Empty, nomeCurso: "", totalAulasCurso: 0, valor: 0m);
            var repositorioMock = new Mock<IAlunoRepository>(MockBehavior.Strict);
            var handler = new MatricularAlunoCursoCommandHandler(repositorioMock.Object);

            // Act
            var resultado = await handler.Handle(comandoInvalido, CancellationToken.None);

            // Assert
            Assert.Same(comandoInvalido.ValidationResult, resultado);
            repositorioMock.Verify(r => r.ObterComMatriculasPorId(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact(DisplayName = "MatricularAlunoCurso quando aluno não encontrado adiciona erro")]
        [Trait("Categoria", "Gestão Aluno - Application - Commands - MatricularAlunoCursoHandler")]
        public async Task Handle_AlunoNaoEncontrado_AdicionaErro()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var comando = new MatricularAlunoCursoCommand(Guid.NewGuid(), alunoId, "Curso X", 10, 100m);

            var repositorioMock = new Mock<IAlunoRepository>();
            repositorioMock.Setup(r => r.ObterComMatriculasPorId(alunoId, It.IsAny<CancellationToken>())).ReturnsAsync((Aluno?)null);

            var handler = new MatricularAlunoCursoCommandHandler(repositorioMock.Object);

            // Act
            var resultado = await handler.Handle(comando, CancellationToken.None);

            // Assert
            Assert.False(resultado.IsValid);
            Assert.Contains(resultado.Errors, e => e.ErrorMessage == "Aluno não encontrado!");
        }

        [Fact(DisplayName = "MatricularAlunoCurso quando aluno já matriculado adiciona erro")]
        [Trait("Categoria", "Gestão Aluno - Application - Commands - MatricularAlunoCursoHandler")]
        public async Task Handle_AlunoJaMatriculado_AdicionaErro()
        {
            // Arrange
            var cursoId = Guid.NewGuid();
            var matriculaExistente = new Matricula(cursoId, "Curso X", totalAulasCurso: 5, valor: 100m);
            _aluno.RealizarMatricula(matriculaExistente);

            var comando = new MatricularAlunoCursoCommand(cursoId, _aluno.Id, "Curso X", totalAulasCurso: 5, valor: 100m);

            var repositorioMock = new Mock<IAlunoRepository>();
            repositorioMock.Setup(r => r.ObterComMatriculasPorId(_aluno.Id, It.IsAny<CancellationToken>())).ReturnsAsync(_aluno);

            var handler = new MatricularAlunoCursoCommandHandler(repositorioMock.Object);

            // Act
            var resultado = await handler.Handle(comando, CancellationToken.None);

            // Assert
            Assert.False(resultado.IsValid);
            Assert.Contains(resultado.Errors, e => e.ErrorMessage == "Aluno já matriculado no curso!");
            repositorioMock.Verify(r => r.RealizarMatricula(It.IsAny<Matricula>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory(DisplayName = "MatricularAlunoCurso quando válido persiste conforme Commit do UnitOfWork")]
        [Trait("Categoria", "Gestão Aluno - Application - Commands - MatricularAlunoCursoHandler")]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Handle_Valido_PersisteConformeCommit(bool commitResult)
        {
            // Arrange
            var cursoId = Guid.NewGuid();
            var comando = new MatricularAlunoCursoCommand(cursoId, _aluno.Id, "Curso X", totalAulasCurso: 3, valor: 200m);

            var uowMock = new Mock<IUnitOfWork>();
            uowMock.Setup(u => u.Commit()).ReturnsAsync(commitResult);

            var repositorioMock = new Mock<IAlunoRepository>();
            repositorioMock.Setup(r => r.ObterComMatriculasPorId(_aluno.Id, It.IsAny<CancellationToken>())).ReturnsAsync(_aluno);
            repositorioMock.SetupGet(r => r.UnitOfWork).Returns(uowMock.Object);

            Matricula? matriculaObtida = null;
            repositorioMock.Setup(r => r.RealizarMatricula(It.IsAny<Matricula>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Callback<Matricula, CancellationToken>((m, ct) => matriculaObtida = m);

            var handler = new MatricularAlunoCursoCommandHandler(repositorioMock.Object);

            // Act
            var resultado = await handler.Handle(comando, CancellationToken.None);

            // Assert
            Assert.Equal(commitResult, resultado.IsValid);
            if (!commitResult)
                Assert.Contains(resultado.Errors, e => e.ErrorMessage == "Houve um erro ao persistir os dados");

            repositorioMock.Verify(r => r.RealizarMatricula(It.IsAny<Matricula>(), It.IsAny<CancellationToken>()), Times.Once);
            repositorioMock.VerifyGet(r => r.UnitOfWork, Times.AtLeastOnce);
            uowMock.Verify(u => u.Commit(), Times.Once);

            Assert.NotNull(matriculaObtida);
            Assert.Equal(cursoId, matriculaObtida!.CursoId);
            Assert.Equal("Curso X", matriculaObtida.NomeCurso);
            Assert.Equal(200m, matriculaObtida.Valor);
        }
    }
}
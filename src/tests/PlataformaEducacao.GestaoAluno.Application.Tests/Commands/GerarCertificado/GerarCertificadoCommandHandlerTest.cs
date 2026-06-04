using Moq;
using PlataformaEducacao.Core.Data;
using PlataformaEducacao.GestaoAluno.Application.Commands.GerarCertificado;
using PlataformaEducacao.GestaoAluno.Domain;
using PlataformaEducacao.GestaoAluno.Domain.Repositories;

namespace PlataformaEducacao.GestaoAluno.Application.Tests.Commands.GerarCertificado
{
    public class GerarCertificadoCommandHandlerTest
    {
        [Fact(DisplayName = "GerarCertificadoCommand quando matrícula não existe deve adicionar erro no persist")]
        [Trait("Categoria", "Gestão Aluno - Application - Commands - GerarCertificadoCommandHandler")]
        public async Task Handle_MatriculaNaoExistente_AdicionaErroPersist()
        {
            // Arrange
            var matriculaId = Guid.NewGuid();
            var comando = new GerarCertificadoCommand(matriculaId);

            var repositorioMock = new Mock<IAlunoRepository>();
            repositorioMock.Setup(r => r.ObterMatriculaComCertificadoPorId(matriculaId, It.IsAny<CancellationToken>())).ReturnsAsync((Matricula?)null);

            var uowMock = new Mock<IUnitOfWork>();
            uowMock.Setup(u => u.Commit()).ReturnsAsync(false);
            repositorioMock.SetupGet(r => r.UnitOfWork).Returns(uowMock.Object);

            var handler = new GerarCertificadoCommandHandler(repositorioMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => handler.Handle(comando, CancellationToken.None));
        }

        [Fact(DisplayName = "GerarCertificadoCommand quando matrícula possui certificado chama repositório e persiste")]
        [Trait("Categoria", "Gestão Aluno - Application - Commands - GerarCertificadoCommandHandler")]
        public async Task Handle_MatriculaComCertificado_ChamaRepositorioEPersiste()
        {
            // Arrange
            var aluno = new Aluno(Guid.NewGuid(), "Aluno Teste", "email@teste.com");
            var matricula = Matricula.MatriculaFactory.CriarComCursoFinalizado(Guid.NewGuid(), "Curso", totalAulasCurso: 1, valor: 100m, aluno);

            var comando = new GerarCertificadoCommand(matricula.Id);

            var repositorioMock = new Mock<IAlunoRepository>();
            repositorioMock.Setup(r => r.ObterMatriculaComCertificadoPorId(matricula.Id, It.IsAny<CancellationToken>())).ReturnsAsync(matricula);

            var uowMock = new Mock<IUnitOfWork>();
            uowMock.Setup(u => u.Commit()).ReturnsAsync(true);
            repositorioMock.SetupGet(r => r.UnitOfWork).Returns(uowMock.Object);

            Certificado? certificadoGerado = null;
            repositorioMock.Setup(r => r.GerarCertificado(It.IsAny<Certificado>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Callback<Certificado, CancellationToken>((c, ct) => certificadoGerado = c);

            var handler = new GerarCertificadoCommandHandler(repositorioMock.Object);

            // Act
            var resultado = await handler.Handle(comando, CancellationToken.None);

            // Assert
            Assert.True(resultado.IsValid);
            repositorioMock.Verify(r => r.GerarCertificado(It.IsAny<Certificado>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.NotNull(certificadoGerado);
            Assert.Equal(matricula.Id, certificadoGerado!.MatriculaId);
            uowMock.Verify(u => u.Commit(), Times.Once);
        }
    }
}
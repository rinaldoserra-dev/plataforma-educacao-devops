using PlataformaEducacao.GestaoAluno.Application.Commands.GerarCertificado;

namespace PlataformaEducacao.GestaoAluno.Application.Tests.Commands.GerarCertificado
{
    public class GerarCertificadoCommandTest
    {
        [Fact(DisplayName = "GerarCertificadoCommand quando MatriculaId for válido deve ser válido")]
        [Trait("Categoria", "Gestão Aluno - Application - Commands - GerarCertificadoCommand")]
        public void GerarCertificadoCommand_QuandoMatriculaIdValido_EhValido()
        {
            // Arrange
            var comando = new GerarCertificadoCommand(Guid.NewGuid());
            var validador = new GerarCertificadoCommandValidation();

            // Act
            var resultado = validador.Validate(comando);

            // Assert
            Assert.True(resultado.IsValid);
        }

        [Fact(DisplayName = "GerarCertificadoCommand quando MatriculaId for vazio deve ser inválido")]
        [Trait("Categoria", "Gestão Aluno - Application - Commands - GerarCertificadoCommand")]
        public void GerarCertificadoCommand_QuandoMatriculaIdVazio_DeveSerInvalido()
        {
            // Arrange
            var comando = new GerarCertificadoCommand(Guid.Empty);
            var validador = new GerarCertificadoCommandValidation();

            // Act
            var resultado = validador.Validate(comando);

            // Assert
            Assert.False(resultado.IsValid);
            Assert.Contains(resultado.Errors, e => e.ErrorMessage == "Id da matrícula inválido.");
        }
    }
}
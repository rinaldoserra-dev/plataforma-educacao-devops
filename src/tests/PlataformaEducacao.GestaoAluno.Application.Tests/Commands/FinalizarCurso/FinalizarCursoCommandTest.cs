using PlataformaEducacao.GestaoAluno.Application.Commands.FinalizarCurso;

namespace PlataformaEducacao.GestaoAluno.Application.Tests.Commands.FinalizarCurso
{
    public class FinalizarCursoCommandTest
    {
        [Fact(DisplayName = "FinalizarCursoCommand quando MatriculaId for válido deve ser válido")]
        [Trait("Categoria", "Gestão Aluno - Application - Commands - FinalizarCursoCommand")]
        public void FinalizarCursoCommand_QuandoMatriculaIdValido_EhValido()
        {
            // Arrange
            var comando = new FinalizarCursoCommand(Guid.NewGuid());

            // Act
            var resultado = comando.EhValido();

            // Assert
            Assert.True(resultado);
        }

        [Fact(DisplayName = "FinalizarCursoCommand quando MatriculaId for vazio deve ser inválido")]
        [Trait("Categoria", "Gestão Aluno - Application - Commands - FinalizarCursoCommand")]
        public void FinalizarCursoCommand_QuandoMatriculaIdVazio_DeveSerInvalido()
        {
            // Arrange
            var comando = new FinalizarCursoCommand(Guid.Empty);

            // Act
            var resultado = comando.EhValido();

            // Assert
            Assert.False(resultado);
            Assert.Contains(comando.ValidationResult.Errors, e => e.ErrorMessage == "Id da matrícula inválido.");
        }
    }
}
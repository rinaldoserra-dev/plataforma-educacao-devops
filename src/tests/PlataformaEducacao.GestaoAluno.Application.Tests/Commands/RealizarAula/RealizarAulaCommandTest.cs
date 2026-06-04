using PlataformaEducacao.GestaoAluno.Application.Commands.RealizarAula;

namespace PlataformaEducacao.GestaoAluno.Application.Tests.Commands.RealizarAula
{
    public class RealizarAulaCommandTest
    {
        [Fact(DisplayName = "RealizarAulaCommand quando ids válidos deve ser válido")]
        [Trait("Categoria", "Gestão Aluno - Application - Commands - RealizarAulaCommand")]
        public void RealizarAulaCommand_IdsValidos_EhValido()
        {
            // Arrange
            var comando = new RealizarAulaCommand(matriculaId: Guid.NewGuid(), cursoId: Guid.NewGuid(), aulaId: Guid.NewGuid());

            // Act
            var resultado = comando.EhValido();

            // Assert
            Assert.True(resultado);
        }

        [Fact(DisplayName = "RealizarAulaCommand quando MatriculaId vazio deve ser inválido")]
        [Trait("Categoria", "Gestão Aluno - Application - Commands - RealizarAulaCommand")]
        public void RealizarAulaCommand_MatriculaIdVazio_DeveSerInvalido()
        {
            // Arrange
            var comando = new RealizarAulaCommand(matriculaId: Guid.Empty, cursoId: Guid.NewGuid(), aulaId: Guid.NewGuid());

            // Act
            var resultado = comando.EhValido();

            // Assert
            Assert.False(resultado);
            Assert.Contains(comando.ValidationResult.Errors, e => e.ErrorMessage == "Id da matrícula inválido.");
        }

        [Fact(DisplayName = "RealizarAulaCommand quando CursoId vazio deve ser inválido")]
        [Trait("Categoria", "Gestão Aluno - Application - Commands - RealizarAulaCommand")]
        public void RealizarAulaCommand_CursoIdVazio_DeveSerInvalido()
        {
            // Arrange
            var comando = new RealizarAulaCommand(matriculaId: Guid.NewGuid(), cursoId: Guid.Empty, aulaId: Guid.NewGuid());

            // Act
            var resultado = comando.EhValido();

            // Assert
            Assert.False(resultado);
            Assert.Contains(comando.ValidationResult.Errors, e => e.ErrorMessage == "Id do curso inválido.");
        }

        [Fact(DisplayName = "RealizarAulaCommand quando AulaId vazio deve ser inválido")]
        [Trait("Categoria", "Gestão Aluno - Application - Commands - RealizarAulaCommand")]
        public void RealizarAulaCommand_AulaIdVazio_DeveSerInvalido()
        {
            // Arrange
            var comando = new RealizarAulaCommand(matriculaId: Guid.NewGuid(), cursoId: Guid.NewGuid(), aulaId: Guid.Empty);

            // Act
            var resultado = comando.EhValido();

            // Assert
            Assert.False(resultado);
            Assert.Contains(comando.ValidationResult.Errors, e => e.ErrorMessage == "Id da aula inválido.");
        }
    }
}
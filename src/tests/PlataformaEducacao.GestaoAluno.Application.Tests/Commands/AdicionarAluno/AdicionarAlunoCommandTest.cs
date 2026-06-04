using PlataformaEducacao.GestaoAluno.Application.Commands.AdicionarAluno;

namespace PlataformaEducacao.GestaoAluno.Application.Tests.Commands.AdicionarAluno
{
    public class AdicionarAlunoCommandTest
    {
        [Fact(DisplayName = "AdicionarAlunoCommand quando dados corretos deve ser válido")]
        [Trait("Categoria", "Gestão Aluno - Application - AdicionarAlunoCommand")]
        public void AdicionarAlunoCommand_QuandoDadosCorretos_EhValido()
        {
            // Arrange
            var command = new AdicionarAlunoCommand(Guid.NewGuid(), "Fulano de Tal", "fulano@teste.com");

            // Act
            var result = command.EhValido();

            // Assert
            Assert.True(result);
        }

        [Fact(DisplayName = "AdicionarAlunoCommand quando UsuarioId for vazio deve ser inválido")]
        [Trait("Categoria", "Gestão Aluno - Application - AdicionarAlunoCommand")]
        public void AdicionarAlunoCommand_QuandoUsuarioIdVazio_DeveSerInvalido()
        {
            // Arrange
            var command = new AdicionarAlunoCommand(Guid.Empty, "Fulano de Tal", "fulano@teste.com");

            // Act
            var result = command.EhValido();

            // Assert
            Assert.False(result);
            Assert.Contains(command.ValidationResult.Errors, e => e.ErrorMessage == "O id do usuário é obrigatório.");
        }

        [Fact(DisplayName = "AdicionarAlunoCommand quando nome do aluno for vazio deve ser inválido")]
        [Trait("Categoria", "Gestão Aluno - Application - AdicionarAlunoCommand")]
        public void AdicionarAlunoCommand_QuandoNomeVazio_DeveSerInvalido()
        {
            // Arrange
            var command = new AdicionarAlunoCommand(Guid.NewGuid(), "", "fulano@teste.com");

            // Act
            var result = command.EhValido();

            // Assert
            Assert.False(result);
            Assert.Contains(command.ValidationResult.Errors, e => e.ErrorMessage == "O nome do aluno é obrigatório.");
        }

        [Fact(DisplayName = "AdicionarAlunoCommand quando e-mail do aluno for vazio deve ser inválido")]
        [Trait("Categoria", "Gestão Aluno - Application - AdicionarAlunoCommand")]
        public void AdicionarAlunoCommand_QuandoEmailVazio_DeveSerInvalido()
        {
            // Arrange
            var command = new AdicionarAlunoCommand(Guid.NewGuid(), "Fulano de Tal", "");

            // Act
            var result = command.EhValido();

            // Assert
            Assert.False(result);
            Assert.Contains(command.ValidationResult.Errors, e => e.ErrorMessage == "O e-mail do aluno é obrigatório.");
        }
    }
}
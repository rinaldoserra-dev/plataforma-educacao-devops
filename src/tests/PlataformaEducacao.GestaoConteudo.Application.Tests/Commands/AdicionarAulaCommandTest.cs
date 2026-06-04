using PlataformaEducacao.GestaoConteudo.Application.Commands;

namespace PlataformaEducacao.GestaoConteudo.Application.Tests.Commands
{
    public class AdicionarAulaCommandTest
    {
        [Fact(DisplayName = "Adicionar Aula Command Válido")]
        [Trait("Categoria", "Gestao Conteudo - AdicionarAulaCommand")]
        public void AdiconarAulaCommand_EhValido_QuandoDadosCorretos()
        {
            // Arrange
            var command = new AdicionarAulaCommand("Aula 1", "Conteudo da aula", 1, "Material", Guid.NewGuid());

            // Act
            var result = command.EhValido();

            // Assert
            Assert.True(result);
        }

        [Fact(DisplayName = "Deve ser inválido quando o titulo é inválido")]
        [Trait("Categoria", "Gestao Conteudo - AdicionarAulaCommand")]
        public void AdicionarAulaCommand_DeveSerInvalido_QuandoTituloVazio()
        {
            // Arrange
            var command = new AdicionarAulaCommand("", "Conteudo da aula", 1, "Material", Guid.NewGuid());

            // Act
            var result = command.EhValido();

            Assert.False(result);
            Assert.Contains(command.ValidationResult.Errors, e => e.ErrorMessage == "Título da aula é obrigatório.");
        }

        [Fact(DisplayName = "Deve ser inválido quando conteudo é inválida")]
        [Trait("Categoria", "Gestao Conteudo - AdicionarAulaCommand")]
        public void AdicionarAulaCommand_DeveSerInvalido_QuandoConteudoVazio()
        {
            // Arrange
            var command = new AdicionarAulaCommand("Aula 1", "  ", 1, "Material", Guid.NewGuid());

            // Act
            var result = command.EhValido();

            Assert.False(result);
            Assert.Contains(command.ValidationResult.Errors, e => e.ErrorMessage == "O conteudo é obrigatório.");
        }

        [Fact(DisplayName = "Deve ser inválido quando a ordem é inválida")]
        [Trait("Categoria", "Gestao Conteudo - AdicionarAulaCommand")]
        public void AdicionarAulaCommand_DeveSerInvalido_QuandoOrdemMenorIgualZero()
        {
            // Arrange
            var command = new AdicionarAulaCommand("Aula 1", "Conteudo da Aula", 0, "Material", Guid.NewGuid());

            // Act
            var result = command.EhValido();

            Assert.False(result);
            Assert.Contains(command.ValidationResult.Errors, e => e.ErrorMessage == "A ordem da aula deve ser maior que 0.");
        }
        [Fact(DisplayName = "Deve ser inválido quando o curso é inválido")]
        [Trait("Categoria", "Gestao Conteudo - AdicionarAulaCommand")]
        public void AdicionarAulaCommand_DeveSerInvalido_QuandoCursoInvalido()
        {
            // Arrange
            var command = new AdicionarAulaCommand("Aula 1", "Conteudo Aula", 1, "Material", Guid.Empty);

            // Act
            var result = command.EhValido();

            Assert.False(result);
            Assert.Contains(command.ValidationResult.Errors, e => e.ErrorMessage == "Curso é obrigatório.");
        }

    }
}

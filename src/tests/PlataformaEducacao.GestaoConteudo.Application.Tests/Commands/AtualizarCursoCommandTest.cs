using PlataformaEducacao.GestaoConteudo.Application.Commands;

namespace PlataformaEducacao.GestaoConteudo.Application.Tests.Commands
{
    public class AtualizarCursoCommandTest
    {
        [Fact(DisplayName = "Atualizar Curso Command Válido")]
        [Trait("Categoria", "Gestao Conteudo - AtualizarCursoCommand")]
        public void AtualizarCursoCommand_EhValido_QuandoDadosCorretos()
        {
            // Arrange
            var command = new AtualizarCursoCommand(Guid.NewGuid(), "Curso C#", "Conteudo do curso", 5, 500, true);

            // Act
            var result = command.EhValido();

            // Assert
            Assert.True(result);
        }

        [Fact(DisplayName = "Deve ser inválido quando o idCurso é inválido")]
        [Trait("Categoria", "Gestao Conteudo - AtualizarCursoCommand")]
        public void AtualizarCursoCommand_DeveSerInvalido_QuandoIdVazio()
        {
            // Arrange
            var command = new AtualizarCursoCommand(Guid.Empty, "Curso C#", "Conteudo do curso", 5, 500, true);

            // Act
            var result = command.EhValido();

            Assert.False(result);
            Assert.Contains(command.ValidationResult.Errors, e => e.ErrorMessage == "Id do curso é obrigatório.");
        }

        [Fact(DisplayName = "Deve ser inválido quando o nome é inválido")]
        [Trait("Categoria", "Gestao Conteudo - AtualizarCursoCommand")]
        public void AtualizarCursoCommand_DeveSerInvalido_QuandoNomeVazio()
        {
            // Arrange
            var command = new AtualizarCursoCommand(Guid.NewGuid(), "", "Conteudo do curso", 5, 500, true);

            // Act
            var result = command.EhValido();

            Assert.False(result);
            Assert.Contains(command.ValidationResult.Errors, e => e.ErrorMessage == "Nome do curso é obrigatório.");
        }

        [Fact(DisplayName = "Deve ser inválido quando a descrição é inválida")]
        [Trait("Categoria", "Gestao Conteudo - AtualizarCursoCommand")]
        public void AtualizarCursoCommand_DeveSerInvalido_QuandoDescricaoConteudoVazio()
        {
            // Arrange
            var command = new AtualizarCursoCommand(Guid.NewGuid(), "Curso C#", "", 5, 500, true);

            // Act
            var result = command.EhValido();

            Assert.False(result);
            Assert.Contains(command.ValidationResult.Errors, e => e.ErrorMessage == "A descrição do conteudo programático é obrigatória.");
        }

        [Fact(DisplayName = "Deve ser inválido quando a carga horária é inválida")]
        [Trait("Categoria", "Gestao Conteudo - AtualizarCursoCommand")]
        public void AtualizarCursoCommand_DeveSerInvalido_QuandoDuracaoMenorIgualZero()
        {
            // Arrange
            var command = new AtualizarCursoCommand(Guid.NewGuid(), "Curso C#", "Conteudo do curso", 0, 500, true);

            // Act
            var result = command.EhValido();

            Assert.False(result);
            Assert.Contains(command.ValidationResult.Errors, e => e.ErrorMessage == "A carga horária do curso deve ser maior que 0.");
        }

        [Fact(DisplayName = "Deve ser inválido quando o valor é inválido")]
        [Trait("Categoria", "Gestao Conteudo - AtualizarCursoCommand")]
        public void AtualizarCursoCommand_DeveSerInvalido_QuandoPrecoMenorIgualZero()
        {
            // Arrange
            var command = new AtualizarCursoCommand(Guid.NewGuid(), "Curso C#", "Conteudo do curso", 5, 0, true);

            // Act
            var result = command.EhValido();

            Assert.False(result);
            Assert.Contains(command.ValidationResult.Errors, e => e.ErrorMessage == "O valor do curso deve ser maior que 0.");
        }
    }
}

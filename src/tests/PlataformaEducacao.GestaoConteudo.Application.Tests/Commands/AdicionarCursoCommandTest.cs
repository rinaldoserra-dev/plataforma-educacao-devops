using Bogus;
using PlataformaEducacao.GestaoConteudo.Application.Commands;

namespace PlataformaEducacao.GestaoConteudo.Application.Tests.Commands
{
    public class AdicionarCursoCommandTest
    {
        private Faker Faker { get; set; } = new Faker("pt_BR");

        [Fact(DisplayName = "Adicionar Curso Command Válido")]
        [Trait("Categoria", "Gestao Conteudo - AdicionarCursoCommand")]
        public void AdiconarCursoCommand_EhValido_QuandoDadosCorretos()
        {
            // Arrange
            var command = new AdicionarCursoCommand("Curso C#", "Conteudo do curso", 5, 500, true);

            // Act
            var result = command.EhValido();

            // Assert
            Assert.True(result);
        }

        [Fact(DisplayName = "Deve ser inválido quando o nome é inválido")]
        [Trait("Categoria", "Gestao Conteudo - AdicionarCursoCommand")]
        public void AdicionarCursoCommand_DeveSerInvalido_QuandoNomeVazio()
        {
            // Arrange
            var command = new AdicionarCursoCommand("", "Conteudo do curso", 5, 500, true);

            // Act
            var result = command.EhValido();

            Assert.False(result);
            Assert.Contains(command.ValidationResult.Errors, e => e.ErrorMessage == "Nome do curso é obrigatório.");
        }

        [Fact(DisplayName = "Deve ser inválido quando o tamanho do campo nome é inválido")]
        [Trait("Categoria", "Gestao Conteudo - AdicionarCursoCommand")]
        public void AdicionarCursoCommand_DeveSerInvalido_QuandoTamanhoCampoNomeInvalido()
        {
            // Arrange
            var nomeInvalido = Faker.Lorem.Letter(256);
            var command = new AdicionarCursoCommand(nomeInvalido, "Conteudo do curso", 5, 500, true);

            // Act
            var result = command.EhValido();

            Assert.False(result);
            Assert.Contains(command.ValidationResult.Errors, e => e.ErrorMessage == "Nome do curso deve ter no máximo 255 caracteres.");
        }

        [Fact(DisplayName = "Deve ser inválido quando a descrição é inválida")]
        [Trait("Categoria", "Gestao Conteudo - AdicionarCursoCommand")]
        public void AdicionarCursoCommand_DeveSerInvalido_QuandoDescricaoConteudoVazio()
        {
            // Arrange
            var command = new AdicionarCursoCommand("Curso C#", "", 5, 500, true);

            // Act
            var result = command.EhValido();

            Assert.False(result);
            Assert.Contains(command.ValidationResult.Errors, e => e.ErrorMessage == "A descrição do conteudo programático é obrigatória.");
        }

        [Fact(DisplayName = "Deve ser inválido quando o tamanho do campo descrição é inválida")]
        [Trait("Categoria", "Gestao Conteudo - AdicionarCursoCommand")]
        public void AdicionarCursoCommand_DeveSerInvalido_QuandoTamanhoCampoDescricaoConteudoInvalido()
        {
            // Arrange
            var descricaoInvalido = Faker.Lorem.Letter(1001);
            var command = new AdicionarCursoCommand("Curso C#", descricaoInvalido, 5, 500, true);

            // Act
            var result = command.EhValido();

            Assert.False(result);
            Assert.Contains(command.ValidationResult.Errors, e => e.ErrorMessage == "A descrição do conteudo programático deve ter no máximo 1000 caracteres.");
        }

        [Fact(DisplayName = "Deve ser inválido quando a carga horária é inválida")]
        [Trait("Categoria", "Gestao Conteudo - AdicionarCursoCommand")]
        public void AdicionarCursoCommand_DeveSerInvalido_QuandoDuracaoMenorIgualZero()
        {
            // Arrange
            var command = new AdicionarCursoCommand("Curso C#", "Conteudo do curso", 0, 500, true);

            // Act
            var result = command.EhValido();

            Assert.False(result);
            Assert.Contains(command.ValidationResult.Errors, e => e.ErrorMessage == "A carga horária do curso deve ser maior que 0.");
        }

        [Fact(DisplayName = "Deve ser inválido quando o valor é inválida")]
        [Trait("Categoria", "Gestao Conteudo - AdicionarCursoCommand")]
        public void AdicionarCursoCommand_DeveSerInvalido_QuandoPrecoMenorIgualZero()
        {
            // Arrange
            var command = new AdicionarCursoCommand("Curso C#", "Conteudo do curso", 5, 0, true);

            // Act
            var result = command.EhValido();

            Assert.False(result);
            Assert.Contains(command.ValidationResult.Errors, e => e.ErrorMessage == "O valor do curso deve ser maior que 0.");
        }
    }
}

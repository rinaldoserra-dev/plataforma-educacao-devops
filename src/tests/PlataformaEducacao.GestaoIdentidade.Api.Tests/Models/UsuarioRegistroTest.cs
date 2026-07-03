using PlataformaEducacao.GestaoIdentidade.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace PlataformaEducacao.GestaoIdentidade.Api.Tests.Models
{
    public class UsuarioRegistroTest
    {
        [Fact(DisplayName = "UsuarioRegistro válido é considerado válido")]
        [Trait("Categoria", "Gestão Identidade - Models - UsuarioRegistro")]
        public void UsuarioRegistro_Valido_RetornaValido()
        {
            // Arrange
            var registro = new UsuarioRegistro
            {
                Nome = "Aluno Teste",
                Email = "aluno@teste.com",
                Senha = "Teste@123",
                SenhaConfirmacao = "Teste@123"
            };
            var contexto = new ValidationContext(registro);
            var resultados = new List<ValidationResult>();

            // Act
            var ehValido = Validator.TryValidateObject(registro, contexto, resultados, validateAllProperties: true);

            // Assert
            Assert.True(ehValido);
            Assert.Empty(resultados);
        }

        [Fact(DisplayName = "UsuarioRegistro sem nome retorna erro")]
        [Trait("Categoria", "Gestão Identidade - Models - UsuarioRegistro")]
        public void UsuarioRegistro_SemNome_RetornaErro()
        {
            // Arrange
            var registro = new UsuarioRegistro
            {
                Nome = "",
                Email = "aluno@teste.com",
                Senha = "Teste@123",
                SenhaConfirmacao = "Teste@123"
            };
            var contexto = new ValidationContext(registro);
            var resultados = new List<ValidationResult>();

            // Act
            var ehValido = Validator.TryValidateObject(registro, contexto, resultados, validateAllProperties: true);

            // Assert
            Assert.False(ehValido);
        }

        [Fact(DisplayName = "UsuarioRegistro com email inválido retorna erro")]
        [Trait("Categoria", "Gestão Identidade - Models - UsuarioRegistro")]
        public void UsuarioRegistro_EmailInvalido_RetornaErro()
        {
            // Arrange
            var registro = new UsuarioRegistro
            {
                Nome = "Aluno",
                Email = "email-invalido",
                Senha = "Teste@123",
                SenhaConfirmacao = "Teste@123"
            };
            var contexto = new ValidationContext(registro);
            var resultados = new List<ValidationResult>();

            // Act
            var ehValido = Validator.TryValidateObject(registro, contexto, resultados, validateAllProperties: true);

            // Assert
            Assert.False(ehValido);
        }

        [Fact(DisplayName = "UsuarioRegistro com senhas diferentes retorna erro")]
        [Trait("Categoria", "Gestão Identidade - Models - UsuarioRegistro")]
        public void UsuarioRegistro_SenhasDiferentes_RetornaErro()
        {
            // Arrange
            var registro = new UsuarioRegistro
            {
                Nome = "Aluno",
                Email = "aluno@teste.com",
                Senha = "Teste@123",
                SenhaConfirmacao = "Outra@456"
            };
            var contexto = new ValidationContext(registro);
            var resultados = new List<ValidationResult>();

            // Act
            var ehValido = Validator.TryValidateObject(registro, contexto, resultados, validateAllProperties: true);

            // Assert
            Assert.False(ehValido);
            var mensagens = resultados.Select(r => r.ErrorMessage).ToList();
            Assert.Contains("As senhas não conferem.", mensagens);
        }

        [Fact(DisplayName = "UsuarioRegistro com senha curta retorna erro")]
        [Trait("Categoria", "Gestão Identidade - Models - UsuarioRegistro")]
        public void UsuarioRegistro_SenhaCurta_RetornaErro()
        {
            // Arrange
            var registro = new UsuarioRegistro
            {
                Nome = "Aluno",
                Email = "aluno@teste.com",
                Senha = "123",
                SenhaConfirmacao = "123"
            };
            var contexto = new ValidationContext(registro);
            var resultados = new List<ValidationResult>();

            // Act
            var ehValido = Validator.TryValidateObject(registro, contexto, resultados, validateAllProperties: true);

            // Assert
            Assert.False(ehValido);
        }
    }
}

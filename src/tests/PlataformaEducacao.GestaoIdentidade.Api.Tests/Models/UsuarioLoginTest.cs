using PlataformaEducacao.GestaoIdentidade.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace PlataformaEducacao.GestaoIdentidade.Api.Tests.Models
{
    public class UsuarioLoginTest
    {
        [Fact(DisplayName = "UsuarioLogin válido é considerado válido")]
        [Trait("Categoria", "Gestão Identidade - Models - UsuarioLogin")]
        public void UsuarioLogin_Valido_RetornaValido()
        {
            // Arrange
            var login = new UsuarioLogin
            {
                Email = "aluno@teste.com",
                Senha = "Teste@123"
            };
            var contexto = new ValidationContext(login);
            var resultados = new List<ValidationResult>();

            // Act
            var ehValido = Validator.TryValidateObject(login, contexto, resultados, validateAllProperties: true);

            // Assert
            Assert.True(ehValido);
            Assert.Empty(resultados);
        }

        [Fact(DisplayName = "UsuarioLogin sem email retorna erro")]
        [Trait("Categoria", "Gestão Identidade - Models - UsuarioLogin")]
        public void UsuarioLogin_SemEmail_RetornaErro()
        {
            // Arrange
            var login = new UsuarioLogin
            {
                Email = "",
                Senha = "Teste@123"
            };
            var contexto = new ValidationContext(login);
            var resultados = new List<ValidationResult>();

            // Act
            var ehValido = Validator.TryValidateObject(login, contexto, resultados, validateAllProperties: true);

            // Assert
            Assert.False(ehValido);
        }

        [Fact(DisplayName = "UsuarioLogin com email inválido retorna erro")]
        [Trait("Categoria", "Gestão Identidade - Models - UsuarioLogin")]
        public void UsuarioLogin_EmailInvalido_RetornaErro()
        {
            // Arrange
            var login = new UsuarioLogin
            {
                Email = "invalido",
                Senha = "Teste@123"
            };
            var contexto = new ValidationContext(login);
            var resultados = new List<ValidationResult>();

            // Act
            var ehValido = Validator.TryValidateObject(login, contexto, resultados, validateAllProperties: true);

            // Assert
            Assert.False(ehValido);
        }

        [Fact(DisplayName = "UsuarioLogin sem senha retorna erro")]
        [Trait("Categoria", "Gestão Identidade - Models - UsuarioLogin")]
        public void UsuarioLogin_SemSenha_RetornaErro()
        {
            // Arrange
            var login = new UsuarioLogin
            {
                Email = "aluno@teste.com",
                Senha = ""
            };
            var contexto = new ValidationContext(login);
            var resultados = new List<ValidationResult>();

            // Act
            var ehValido = Validator.TryValidateObject(login, contexto, resultados, validateAllProperties: true);

            // Assert
            Assert.False(ehValido);
        }

        [Fact(DisplayName = "UsuarioLogin com senha curta retorna erro")]
        [Trait("Categoria", "Gestão Identidade - Models - UsuarioLogin")]
        public void UsuarioLogin_SenhaCurta_RetornaErro()
        {
            // Arrange
            var login = new UsuarioLogin
            {
                Email = "aluno@teste.com",
                Senha = "12"
            };
            var contexto = new ValidationContext(login);
            var resultados = new List<ValidationResult>();

            // Act
            var ehValido = Validator.TryValidateObject(login, contexto, resultados, validateAllProperties: true);

            // Assert
            Assert.False(ehValido);
        }
    }
}

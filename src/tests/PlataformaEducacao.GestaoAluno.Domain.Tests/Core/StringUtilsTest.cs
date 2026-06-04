using FluentAssertions;
using PlataformaEducacao.Core.Utils;

namespace PlataformaEducacao.GestaoAluno.Domain.Tests.Core
{
    public class StringUtilsTest
    {
        [Fact(DisplayName = "ApenasNumeros deve retornar somente dígitos")]
        [Trait("Categoria", "Core - Utils - StringUtils")]
        public void ApenasNumeros_ComLetrasENumeros_DeveRetornarSomenteDigitos()
        {
            // Arrange & Act
            var resultado = string.Empty.ApenasNumeros("abc123def456");

            // Assert
            resultado.Should().Be("123456");
        }

        [Fact(DisplayName = "ApenasNumeros com string sem números deve retornar vazio")]
        [Trait("Categoria", "Core - Utils - StringUtils")]
        public void ApenasNumeros_SemNumeros_DeveRetornarVazio()
        {
            // Arrange & Act
            var resultado = string.Empty.ApenasNumeros("abcdef");

            // Assert
            resultado.Should().BeEmpty();
        }

        [Fact(DisplayName = "ApenasNumeros com string vazia deve retornar vazio")]
        [Trait("Categoria", "Core - Utils - StringUtils")]
        public void ApenasNumeros_StringVazia_DeveRetornarVazio()
        {
            // Arrange & Act
            var resultado = string.Empty.ApenasNumeros(string.Empty);

            // Assert
            resultado.Should().BeEmpty();
        }

        [Fact(DisplayName = "ApenasNumeros com somente números deve retornar mesma string")]
        [Trait("Categoria", "Core - Utils - StringUtils")]
        public void ApenasNumeros_SomenteNumeros_DeveRetornarMesmaString()
        {
            // Arrange & Act
            var resultado = string.Empty.ApenasNumeros("123456");

            // Assert
            resultado.Should().Be("123456");
        }

        [Fact(DisplayName = "ApenasNumeros com caracteres especiais deve retornar somente dígitos")]
        [Trait("Categoria", "Core - Utils - StringUtils")]
        public void ApenasNumeros_ComCaracteresEspeciais_DeveRetornarSomenteDigitos()
        {
            // Arrange & Act
            var resultado = string.Empty.ApenasNumeros("(11) 99999-0000");

            // Assert
            resultado.Should().Be("11999990000");
        }
    }
}

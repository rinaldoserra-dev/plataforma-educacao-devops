using System;
using Microsoft.Extensions.DependencyInjection;
using PlataformaEducacao.MessageBus;
using Xunit;

namespace PlataformaEducacao.MessageBus.Tests
{
    public class DependencyInjectionExtensionsTest
    {
        [Trait("Categoria", "Building Blocks - MessageBus")]
        [Theory(DisplayName = "AddMessageBus Quando Conexão For Nula Ou Vazia Deve Lançar ArgumentNullException")]
        [InlineData(null)]
        [InlineData("")]
        public void AddMessageBus_QuandoConexaoForNulaOuVazia_DeveLancarArgumentNullException(string? connection)
        {
            // Arrange
            var services = new ServiceCollection();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => services.AddMessageBus(connection!));
        }
    }
}

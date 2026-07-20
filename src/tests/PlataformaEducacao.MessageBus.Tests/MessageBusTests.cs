using System.Runtime.CompilerServices;
using EasyNetQ;
using Moq;
using PlataformaEducacao.Core.Messages.Integration;

namespace PlataformaEducacao.MessageBus.Tests
{
    public class MessageBusTests
    {
        [Fact(DisplayName = "IsConnected DeveRetornarValorDoBus")]
        [Trait("Categoria", "Building Blocks - MessageBus")]
        public void IsConnected_DeveRetornarValorDoBus()
        {
            // Arrange
            var mockBus = new Mock<IBus>();
            mockBus.SetupGet(b => b.IsConnected).Returns(true);

            var instancia = (MessageBus)RuntimeHelpers.GetUninitializedObject(typeof(MessageBus));
            var campoBus = typeof(MessageBus).GetField("_bus", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
            campoBus.SetValue(instancia, mockBus.Object);

            // Act
            var conectado = instancia.IsConnected;

            // Assert
            Assert.True(conectado);
        }

        [Fact(DisplayName = "AdvancedBus DeveRetornarAdvancedDoIBus")]
        [Trait("Categoria", "Building Blocks - MessageBus")]
        public void AdvancedBus_DeveRetornarAdvancedDoIBus()
        {
            // Arrange
            var mockAdvanced = new Mock<IAdvancedBus>();
            var mockBus = new Mock<IBus>();
            mockBus.SetupGet(b => b.Advanced).Returns(mockAdvanced.Object);

            var instancia = (MessageBus)RuntimeHelpers.GetUninitializedObject(typeof(MessageBus));
            var campoBus = typeof(MessageBus).GetField("_bus", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
            campoBus.SetValue(instancia, mockBus.Object);

            // Act
            var advanced = instancia.AdvancedBus;

            // Assert
            Assert.Equal(mockAdvanced.Object, advanced);
        }

        [Fact(DisplayName = "Dispose DeveChamarDisposeDoBus")]
        [Trait("Categoria", "Building Blocks - MessageBus")]
        public void Dispose_DeveChamarDisposeDoBus()
        {
            // Arrange
            var mockBus = new Mock<IBus>();
            mockBus.Setup(b => b.Dispose());

            var instancia = (MessageBus)RuntimeHelpers.GetUninitializedObject(typeof(MessageBus));
            var campoBus = typeof(MessageBus).GetField("_bus", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
            campoBus.SetValue(instancia, mockBus.Object);

            // Act
            instancia.Dispose();

            // Assert
            mockBus.Verify(b => b.Dispose(), Times.Once);
        }

        private class EventoTeste : IntegrationEvent { }
    }
}

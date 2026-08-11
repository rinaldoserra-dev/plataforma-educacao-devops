using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using EasyNetQ;
using FluentValidation.Results;
using Moq;
using PlataformaEducacao.Core.Messages.Integration;
using Xunit;

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

        [Fact(DisplayName = "Publish DeveDelegarParaBus")]
        [Trait("Categoria", "Building Blocks - MessageBus")]
        public void Publish_DeveDelegarParaBus()
        {
            // Arrange
            var message = new EventoTeste();
            var mockBus = new Mock<IBus>();
            mockBus.SetupGet(b => b.IsConnected).Returns(true);
            mockBus.Setup(b => b.Publish(It.IsAny<EventoTeste>()));

            var instancia = (MessageBus)RuntimeHelpers.GetUninitializedObject(typeof(MessageBus));
            typeof(MessageBus).GetField("_bus", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.SetValue(instancia, mockBus.Object);

            // Act
            instancia.Publish(message);

            // Assert
            mockBus.Verify(b => b.Publish(It.Is<EventoTeste>(m => m == message)), Times.Once);
        }

        [Fact(DisplayName = "PublishAsync DeveDelegarParaBus")]
        [Trait("Categoria", "Building Blocks - MessageBus")]
        public async Task PublishAsync_DeveDelegarParaBus()
        {
            // Arrange
            var message = new EventoTeste();
            var mockBus = new Mock<IBus>();
            mockBus.SetupGet(b => b.IsConnected).Returns(true);
            mockBus.Setup(b => b.PublishAsync(It.IsAny<EventoTeste>())).Returns(Task.CompletedTask);

            var instancia = (MessageBus)RuntimeHelpers.GetUninitializedObject(typeof(MessageBus));
            typeof(MessageBus).GetField("_bus", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.SetValue(instancia, mockBus.Object);

            // Act
            await instancia.PublishAsync(message);

            // Assert
            mockBus.Verify(b => b.PublishAsync(It.Is<EventoTeste>(m => m == message)), Times.Once);
        }

        [Fact(DisplayName = "Subscribe DeveDelegarParaBus")]
        [Trait("Categoria", "Building Blocks - MessageBus")]
        public void Subscribe_DeveDelegarParaBus()
        {
            // Arrange
            var mockBus = new Mock<IBus>();
            mockBus.SetupGet(b => b.IsConnected).Returns(true);
            mockBus.Setup(b => b.Subscribe(It.IsAny<string>(), It.IsAny<Action<EventoTeste>>()));

            var instancia = (MessageBus)RuntimeHelpers.GetUninitializedObject(typeof(MessageBus));
            typeof(MessageBus).GetField("_bus", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.SetValue(instancia, mockBus.Object);

            // Act
            instancia.Subscribe<EventoTeste>("sub", _ => { });

            // Assert
            mockBus.Verify(b => b.Subscribe(It.Is<string>(s => s == "sub"), It.IsAny<Action<EventoTeste>>()), Times.Once);
        }

        [Fact(DisplayName = "SubscribeAsync DeveDelegarParaBus")]
        [Trait("Categoria", "Building Blocks - MessageBus")]
        public void SubscribeAsync_DeveDelegarParaBus()
        {
            // Arrange
            var mockBus = new Mock<IBus>();
            mockBus.SetupGet(b => b.IsConnected).Returns(true);
            mockBus.Setup(b => b.SubscribeAsync(It.IsAny<string>(), It.IsAny<Func<EventoTeste, Task>>()));

            var instancia = (MessageBus)RuntimeHelpers.GetUninitializedObject(typeof(MessageBus));
            typeof(MessageBus).GetField("_bus", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.SetValue(instancia, mockBus.Object);

            // Act
            instancia.SubscribeAsync<EventoTeste>("sub", _ => Task.CompletedTask);

            // Assert
            mockBus.Verify(b => b.SubscribeAsync(It.Is<string>(s => s == "sub"), It.IsAny<Func<EventoTeste, Task>>()), Times.Once);
        }

        [Fact(DisplayName = "Request DeveDelegarParaBusERetornarResposta")]
        [Trait("Categoria", "Building Blocks - MessageBus")]
        public void Request_DeveDelegarParaBusERetornarResposta()
        {
            // Arrange
            var request = new EventoTeste();
            var response = new RespostaTeste();
            var mockBus = new Mock<IBus>();
            mockBus.SetupGet(b => b.IsConnected).Returns(true);
            mockBus.Setup(b => b.Request<EventoTeste, RespostaTeste>(It.IsAny<EventoTeste>())).Returns(response);

            var instancia = (MessageBus)RuntimeHelpers.GetUninitializedObject(typeof(MessageBus));
            typeof(MessageBus).GetField("_bus", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.SetValue(instancia, mockBus.Object);

            // Act
            var resultado = instancia.Request<EventoTeste, RespostaTeste>(request);

            // Assert
            Assert.Equal(response, resultado);
            mockBus.Verify(b => b.Request<EventoTeste, RespostaTeste>(It.Is<EventoTeste>(r => r == request)), Times.Once);
        }

        [Fact(DisplayName = "RequestAsync DeveDelegarParaBusERetornarResposta")]
        [Trait("Categoria", "Building Blocks - MessageBus")]
        public async Task RequestAsync_DeveDelegarParaBusERetornarResposta()
        {
            // Arrange
            var request = new EventoTeste();
            var response = new RespostaTeste();
            var mockBus = new Mock<IBus>();
            mockBus.SetupGet(b => b.IsConnected).Returns(true);
            mockBus.Setup(b => b.RequestAsync<EventoTeste, RespostaTeste>(It.IsAny<EventoTeste>())).ReturnsAsync(response);

            var instancia = (MessageBus)RuntimeHelpers.GetUninitializedObject(typeof(MessageBus));
            typeof(MessageBus).GetField("_bus", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.SetValue(instancia, mockBus.Object);

            // Act
            var resultado = await instancia.RequestAsync<EventoTeste, RespostaTeste>(request);

            // Assert
            Assert.Equal(response, resultado);
            mockBus.Verify(b => b.RequestAsync<EventoTeste, RespostaTeste>(It.Is<EventoTeste>(r => r == request)), Times.Once);
        }

        [Fact(DisplayName = "Respond DeveDelegarParaBusERetornarDisposable")]
        [Trait("Categoria", "Building Blocks - MessageBus")]
        public void Respond_DeveDelegarParaBusERetornarDisposable()
        {
            // Arrange
            var disposableMock = new Mock<IDisposable>();
            var mockBus = new Mock<IBus>();
            mockBus.SetupGet(b => b.IsConnected).Returns(true);
            mockBus.Setup(b => b.Respond(It.IsAny<Func<EventoTeste, RespostaTeste>>()))
                   .Returns(disposableMock.Object);

            var instancia = (MessageBus)RuntimeHelpers.GetUninitializedObject(typeof(MessageBus));
            typeof(MessageBus).GetField("_bus", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.SetValue(instancia, mockBus.Object);

            // Act
            var disposable = instancia.Respond<EventoTeste, RespostaTeste>(_ => new RespostaTeste());

            // Assert
            Assert.Equal(disposableMock.Object, disposable);
            mockBus.Verify(b => b.Respond(It.IsAny<Func<EventoTeste, RespostaTeste>>()), Times.Once);
        }

        [Fact(DisplayName = "RespondAsync DeveDelegarParaBusERetornarDisposable")]
        [Trait("Categoria", "Building Blocks - MessageBus")]
        public void RespondAsync_DeveDelegarParaBusERetornarDisposable()
        {
            // Arrange
            var disposableMock = new Mock<IDisposable>();
            var mockBus = new Mock<IBus>();
            mockBus.SetupGet(b => b.IsConnected).Returns(true);
            mockBus.Setup(b => b.RespondAsync(It.IsAny<Func<EventoTeste, Task<RespostaTeste>>>()))
                   .Returns(disposableMock.Object);

            var instancia = (MessageBus)RuntimeHelpers.GetUninitializedObject(typeof(MessageBus));
            typeof(MessageBus).GetField("_bus", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.SetValue(instancia, mockBus.Object);

            // Act
            var disposable = instancia.RespondAsync<EventoTeste, RespostaTeste>(_ => Task.FromResult(new RespostaTeste()));

            // Assert
            Assert.Equal(disposableMock.Object, disposable);
            mockBus.Verify(b => b.RespondAsync(It.IsAny<Func<EventoTeste, Task<RespostaTeste>>>()), Times.Once);
        }

        private sealed class EventoTeste : IntegrationEvent
        {
        }

        private sealed class RespostaTeste : ResponseMessage
        {
            public RespostaTeste()
                : base(new ValidationResult())
            {
            }
        }
    }
}

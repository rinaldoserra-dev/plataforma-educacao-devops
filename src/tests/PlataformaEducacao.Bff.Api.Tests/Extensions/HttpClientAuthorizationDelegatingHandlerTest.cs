using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using PlataformaEducacao.Bff.Api.Extensions;
using PlataformaEducacao.WebApi.Core.Usuario;

namespace PlataformaEducacao.Bff.Api.Tests.Extensions
{
    public class HttpClientAuthorizationDelegatingHandlerTest
    {
        [Fact(DisplayName = "SendAsync com header Authorization deve propagar para a requisição")]
        [Trait("Categoria", "Bff.Api - Extensions - HttpClientAuthorizationDelegatingHandler")]
        public async Task SendAsync_ComAuthorization_DevePropagarHeader()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = "Bearer meu-token";

            var mockUser = new Mock<IAspNetUser>();
            mockUser.Setup(u => u.ObterHttpContext()).Returns(httpContext);

            var innerHandler = new CaptureRequestHandler();
            var handler = new HttpClientAuthorizationDelegatingHandler(mockUser.Object)
            {
                InnerHandler = innerHandler
            };

            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri("http://localhost")
            };

            // Act
            await client.GetAsync("/api/teste");

            // Assert
            innerHandler.CapturedRequest.Should().NotBeNull();
            innerHandler.CapturedRequest!.Headers.GetValues("Authorization")
                .Should().Contain("Bearer meu-token");
        }

        [Fact(DisplayName = "SendAsync sem header Authorization não deve adicionar header")]
        [Trait("Categoria", "Bff.Api - Extensions - HttpClientAuthorizationDelegatingHandler")]
        public async Task SendAsync_SemAuthorization_NaoDeveAdicionarHeader()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();

            var mockUser = new Mock<IAspNetUser>();
            mockUser.Setup(u => u.ObterHttpContext()).Returns(httpContext);

            var innerHandler = new CaptureRequestHandler();
            var handler = new HttpClientAuthorizationDelegatingHandler(mockUser.Object)
            {
                InnerHandler = innerHandler
            };

            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri("http://localhost")
            };

            // Act
            await client.GetAsync("/api/teste");

            // Assert
            innerHandler.CapturedRequest.Should().NotBeNull();
            innerHandler.CapturedRequest!.Headers.Contains("Authorization").Should().BeFalse();
        }

        private class CaptureRequestHandler : HttpMessageHandler
        {
            public HttpRequestMessage? CapturedRequest { get; private set; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                CapturedRequest = request;
                return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
            }
        }
    }
}

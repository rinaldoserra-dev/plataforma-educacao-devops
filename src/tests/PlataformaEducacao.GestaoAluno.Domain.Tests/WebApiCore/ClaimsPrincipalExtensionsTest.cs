using FluentAssertions;
using PlataformaEducacao.WebApi.Core.Usuario;
using System.Security.Claims;

namespace PlataformaEducacao.GestaoAluno.Domain.Tests.WebApiCore
{
    public class ClaimsPrincipalExtensionsTest
    {
        [Fact(DisplayName = "GetUserId deve retornar valor do claim NameIdentifier")]
        [Trait("Categoria", "WebApi.Core - Usuario - ClaimsPrincipalExtensions")]
        public void GetUserId_ComClaim_DeveRetornarValor()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var principal = CriarPrincipal(new Claim(ClaimTypes.NameIdentifier, userId));

            // Act
            var resultado = principal.GetUserId();

            // Assert
            resultado.Should().Be(userId);
        }

        [Fact(DisplayName = "GetUserId sem claim deve retornar vazio")]
        [Trait("Categoria", "WebApi.Core - Usuario - ClaimsPrincipalExtensions")]
        public void GetUserId_SemClaim_DeveRetornarVazio()
        {
            // Arrange
            var principal = CriarPrincipal();

            // Act
            var resultado = principal.GetUserId();

            // Assert
            resultado.Should().BeEmpty();
        }

        [Fact(DisplayName = "GetUserId com principal nulo deve lançar ArgumentNullException")]
        [Trait("Categoria", "WebApi.Core - Usuario - ClaimsPrincipalExtensions")]
        public void GetUserId_PrincipalNulo_DeveLancarArgumentNullException()
        {
            // Arrange
            ClaimsPrincipal principal = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => principal.GetUserId());
        }

        [Fact(DisplayName = "GetUserEmail deve retornar valor do claim email")]
        [Trait("Categoria", "WebApi.Core - Usuario - ClaimsPrincipalExtensions")]
        public void GetUserEmail_ComClaim_DeveRetornarValor()
        {
            // Arrange
            var principal = CriarPrincipal(new Claim("email", "teste@teste.com"));

            // Act
            var resultado = principal.GetUserEmail();

            // Assert
            resultado.Should().Be("teste@teste.com");
        }

        [Fact(DisplayName = "GetUserEmail sem claim deve retornar vazio")]
        [Trait("Categoria", "WebApi.Core - Usuario - ClaimsPrincipalExtensions")]
        public void GetUserEmail_SemClaim_DeveRetornarVazio()
        {
            // Arrange
            var principal = CriarPrincipal();

            // Act
            var resultado = principal.GetUserEmail();

            // Assert
            resultado.Should().BeEmpty();
        }

        [Fact(DisplayName = "GetUserEmail com principal nulo deve lançar ArgumentNullException")]
        [Trait("Categoria", "WebApi.Core - Usuario - ClaimsPrincipalExtensions")]
        public void GetUserEmail_PrincipalNulo_DeveLancarArgumentNullException()
        {
            ClaimsPrincipal principal = null!;
            Assert.Throws<ArgumentNullException>(() => principal.GetUserEmail());
        }

        [Fact(DisplayName = "GetUserToken deve retornar valor do claim JWT")]
        [Trait("Categoria", "WebApi.Core - Usuario - ClaimsPrincipalExtensions")]
        public void GetUserToken_ComClaim_DeveRetornarValor()
        {
            // Arrange
            var token = "meu-token-jwt";
            var principal = CriarPrincipal(new Claim("JWT", token));

            // Act
            var resultado = principal.GetUserToken();

            // Assert
            resultado.Should().Be(token);
        }

        [Fact(DisplayName = "GetUserToken sem claim deve retornar vazio")]
        [Trait("Categoria", "WebApi.Core - Usuario - ClaimsPrincipalExtensions")]
        public void GetUserToken_SemClaim_DeveRetornarVazio()
        {
            var principal = CriarPrincipal();
            principal.GetUserToken().Should().BeEmpty();
        }

        [Fact(DisplayName = "GetUserToken com principal nulo deve lançar ArgumentNullException")]
        [Trait("Categoria", "WebApi.Core - Usuario - ClaimsPrincipalExtensions")]
        public void GetUserToken_PrincipalNulo_DeveLancarArgumentNullException()
        {
            ClaimsPrincipal principal = null!;
            Assert.Throws<ArgumentNullException>(() => principal.GetUserToken());
        }

        [Fact(DisplayName = "GetUserRefreshToken deve retornar valor do claim RefreshToken")]
        [Trait("Categoria", "WebApi.Core - Usuario - ClaimsPrincipalExtensions")]
        public void GetUserRefreshToken_ComClaim_DeveRetornarValor()
        {
            // Arrange
            var refreshToken = "meu-refresh-token";
            var principal = CriarPrincipal(new Claim("RefreshToken", refreshToken));

            // Act
            var resultado = principal.GetUserRefreshToken();

            // Assert
            resultado.Should().Be(refreshToken);
        }

        [Fact(DisplayName = "GetUserRefreshToken sem claim deve retornar vazio")]
        [Trait("Categoria", "WebApi.Core - Usuario - ClaimsPrincipalExtensions")]
        public void GetUserRefreshToken_SemClaim_DeveRetornarVazio()
        {
            var principal = CriarPrincipal();
            principal.GetUserRefreshToken().Should().BeEmpty();
        }

        [Fact(DisplayName = "GetUserRefreshToken com principal nulo deve lançar ArgumentNullException")]
        [Trait("Categoria", "WebApi.Core - Usuario - ClaimsPrincipalExtensions")]
        public void GetUserRefreshToken_PrincipalNulo_DeveLancarArgumentNullException()
        {
            ClaimsPrincipal principal = null!;
            Assert.Throws<ArgumentNullException>(() => principal.GetUserRefreshToken());
        }

        private static ClaimsPrincipal CriarPrincipal(params Claim[] claims)
        {
            var identity = new ClaimsIdentity(claims, "Test");
            return new ClaimsPrincipal(identity);
        }
    }
}

using FluentAssertions;
using Microsoft.AspNetCore.Http;
using PlataformaEducacao.WebApi.Core.Usuario;
using System.Security.Claims;

namespace PlataformaEducacao.GestaoAluno.Domain.Tests.WebApiCore
{
    public class AspNetUserTest
    {
        [Fact(DisplayName = "ObterUserId autenticado deve retornar Guid do usuário")]
        [Trait("Categoria", "WebApi.Core - Usuario - AspNetUser")]
        public void ObterUserId_Autenticado_DeveRetornarGuid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var aspNetUser = CriarAspNetUser(userId, autenticado: true);

            // Act
            var resultado = aspNetUser.ObterUserId();

            // Assert
            resultado.Should().Be(userId);
        }

        [Fact(DisplayName = "ObterUserId não autenticado deve retornar Guid.Empty")]
        [Trait("Categoria", "WebApi.Core - Usuario - AspNetUser")]
        public void ObterUserId_NaoAutenticado_DeveRetornarGuidEmpty()
        {
            // Arrange
            var aspNetUser = CriarAspNetUser(Guid.NewGuid(), autenticado: false);

            // Act
            var resultado = aspNetUser.ObterUserId();

            // Assert
            resultado.Should().Be(Guid.Empty);
        }

        [Fact(DisplayName = "ObterUserEmail autenticado deve retornar email")]
        [Trait("Categoria", "WebApi.Core - Usuario - AspNetUser")]
        public void ObterUserEmail_Autenticado_DeveRetornarEmail()
        {
            // Arrange
            var aspNetUser = CriarAspNetUser(Guid.NewGuid(), autenticado: true, email: "teste@teste.com");

            // Act
            var resultado = aspNetUser.ObterUserEmail();

            // Assert
            resultado.Should().Be("teste@teste.com");
        }

        [Fact(DisplayName = "ObterUserEmail não autenticado deve retornar vazio")]
        [Trait("Categoria", "WebApi.Core - Usuario - AspNetUser")]
        public void ObterUserEmail_NaoAutenticado_DeveRetornarVazio()
        {
            // Arrange
            var aspNetUser = CriarAspNetUser(Guid.NewGuid(), autenticado: false);

            // Act
            var resultado = aspNetUser.ObterUserEmail();

            // Assert
            resultado.Should().BeEmpty();
        }

        [Fact(DisplayName = "ObterUserToken autenticado deve retornar token")]
        [Trait("Categoria", "WebApi.Core - Usuario - AspNetUser")]
        public void ObterUserToken_Autenticado_DeveRetornarToken()
        {
            // Arrange
            var aspNetUser = CriarAspNetUser(Guid.NewGuid(), autenticado: true, token: "meu-jwt");

            // Act
            var resultado = aspNetUser.ObterUserToken();

            // Assert
            resultado.Should().Be("meu-jwt");
        }

        [Fact(DisplayName = "ObterUserToken não autenticado deve retornar vazio")]
        [Trait("Categoria", "WebApi.Core - Usuario - AspNetUser")]
        public void ObterUserToken_NaoAutenticado_DeveRetornarVazio()
        {
            // Arrange
            var aspNetUser = CriarAspNetUser(Guid.NewGuid(), autenticado: false);

            // Act
            var resultado = aspNetUser.ObterUserToken();

            // Assert
            resultado.Should().BeEmpty();
        }

        [Fact(DisplayName = "ObterUserRefreshToken autenticado deve retornar refresh token")]
        [Trait("Categoria", "WebApi.Core - Usuario - AspNetUser")]
        public void ObterUserRefreshToken_Autenticado_DeveRetornarRefreshToken()
        {
            // Arrange
            var aspNetUser = CriarAspNetUser(Guid.NewGuid(), autenticado: true, refreshToken: "meu-refresh");

            // Act
            var resultado = aspNetUser.ObterUserRefreshToken();

            // Assert
            resultado.Should().Be("meu-refresh");
        }

        [Fact(DisplayName = "ObterUserRefreshToken não autenticado deve retornar vazio")]
        [Trait("Categoria", "WebApi.Core - Usuario - AspNetUser")]
        public void ObterUserRefreshToken_NaoAutenticado_DeveRetornarVazio()
        {
            // Arrange
            var aspNetUser = CriarAspNetUser(Guid.NewGuid(), autenticado: false);

            // Act
            var resultado = aspNetUser.ObterUserRefreshToken();

            // Assert
            resultado.Should().BeEmpty();
        }

        [Fact(DisplayName = "EstaAutenticado quando autenticado deve retornar true")]
        [Trait("Categoria", "WebApi.Core - Usuario - AspNetUser")]
        public void EstaAutenticado_QuandoAutenticado_DeveRetornarTrue()
        {
            // Arrange
            var aspNetUser = CriarAspNetUser(Guid.NewGuid(), autenticado: true);

            // Act & Assert
            aspNetUser.EstaAutenticado().Should().BeTrue();
        }

        [Fact(DisplayName = "EstaAutenticado quando não autenticado deve retornar false")]
        [Trait("Categoria", "WebApi.Core - Usuario - AspNetUser")]
        public void EstaAutenticado_QuandoNaoAutenticado_DeveRetornarFalse()
        {
            // Arrange
            var aspNetUser = CriarAspNetUser(Guid.NewGuid(), autenticado: false);

            // Act & Assert
            aspNetUser.EstaAutenticado().Should().BeFalse();
        }

        [Fact(DisplayName = "PossuiRole deve retornar true quando possui a role")]
        [Trait("Categoria", "WebApi.Core - Usuario - AspNetUser")]
        public void PossuiRole_QuandoPossui_DeveRetornarTrue()
        {
            // Arrange
            var aspNetUser = CriarAspNetUser(Guid.NewGuid(), autenticado: true, role: "ADMIN");

            // Act & Assert
            aspNetUser.PossuiRole("ADMIN").Should().BeTrue();
        }

        [Fact(DisplayName = "PossuiRole deve retornar false quando não possui a role")]
        [Trait("Categoria", "WebApi.Core - Usuario - AspNetUser")]
        public void PossuiRole_QuandoNaoPossui_DeveRetornarFalse()
        {
            // Arrange
            var aspNetUser = CriarAspNetUser(Guid.NewGuid(), autenticado: true, role: "ALUNO");

            // Act & Assert
            aspNetUser.PossuiRole("ADMIN").Should().BeFalse();
        }

        [Fact(DisplayName = "ObterClaims deve retornar claims do usuário")]
        [Trait("Categoria", "WebApi.Core - Usuario - AspNetUser")]
        public void ObterClaims_DeveRetornarClaims()
        {
            // Arrange
            var aspNetUser = CriarAspNetUser(Guid.NewGuid(), autenticado: true);

            // Act
            var claims = aspNetUser.ObterClaims();

            // Assert
            claims.Should().NotBeEmpty();
        }

        [Fact(DisplayName = "ObterHttpContext deve retornar o HttpContext")]
        [Trait("Categoria", "WebApi.Core - Usuario - AspNetUser")]
        public void ObterHttpContext_DeveRetornarHttpContext()
        {
            // Arrange
            var aspNetUser = CriarAspNetUser(Guid.NewGuid(), autenticado: true);

            // Act
            var httpContext = aspNetUser.ObterHttpContext();

            // Assert
            httpContext.Should().NotBeNull();
        }

        [Fact(DisplayName = "Name deve retornar nome do usuário autenticado")]
        [Trait("Categoria", "WebApi.Core - Usuario - AspNetUser")]
        public void Name_Autenticado_DeveRetornarNome()
        {
            // Arrange
            var aspNetUser = CriarAspNetUser(Guid.NewGuid(), autenticado: true, nome: "Usuario Teste");

            // Act & Assert
            aspNetUser.Name.Should().Be("Usuario Teste");
        }

        private static AspNetUser CriarAspNetUser(Guid userId, bool autenticado, string? email = null, string? token = null, string? refreshToken = null, string? role = null, string? nome = null)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId.ToString())
            };

            if (email != null)
                claims.Add(new Claim("email", email));

            if (token != null)
                claims.Add(new Claim("JWT", token));

            if (refreshToken != null)
                claims.Add(new Claim("RefreshToken", refreshToken));

            if (role != null)
                claims.Add(new Claim(ClaimTypes.Role, role));

            if (nome != null)
                claims.Add(new Claim(ClaimTypes.Name, nome));

            var identity = autenticado
                ? new ClaimsIdentity(claims, "Test")
                : new ClaimsIdentity(claims);

            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(identity)
            };

            var accessor = new HttpContextAccessor { HttpContext = httpContext };
            return new AspNetUser(accessor);
        }
    }
}

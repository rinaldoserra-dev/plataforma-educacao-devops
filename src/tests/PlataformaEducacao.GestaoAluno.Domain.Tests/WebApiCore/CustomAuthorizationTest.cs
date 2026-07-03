using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using PlataformaEducacao.WebApi.Core.Identidade;
using System.Security.Claims;

namespace PlataformaEducacao.GestaoAluno.Domain.Tests.WebApiCore
{
    public class CustomAuthorizationTest
    {
        #region ValidarClaimsUsuario

        [Fact(DisplayName = "ValidarClaimsUsuario autenticado com claim correta deve retornar true")]
        [Trait("Categoria", "WebApi.Core - Identidade - CustomAuthorization")]
        public void ValidarClaimsUsuario_AutenticadoComClaim_DeveRetornarTrue()
        {
            // Arrange
            var context = CriarHttpContext(autenticado: true, claimType: "Permissao", claimValue: "Ler");

            // Act
            var resultado = CustomAuthorization.ValidarClaimsUsuario(context, "Permissao", "Ler");

            // Assert
            resultado.Should().BeTrue();
        }

        [Fact(DisplayName = "ValidarClaimsUsuario não autenticado deve retornar false")]
        [Trait("Categoria", "WebApi.Core - Identidade - CustomAuthorization")]
        public void ValidarClaimsUsuario_NaoAutenticado_DeveRetornarFalse()
        {
            // Arrange
            var context = CriarHttpContext(autenticado: false, claimType: "Permissao", claimValue: "Ler");

            // Act
            var resultado = CustomAuthorization.ValidarClaimsUsuario(context, "Permissao", "Ler");

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact(DisplayName = "ValidarClaimsUsuario autenticado sem claim correta deve retornar false")]
        [Trait("Categoria", "WebApi.Core - Identidade - CustomAuthorization")]
        public void ValidarClaimsUsuario_AutenticadoSemClaim_DeveRetornarFalse()
        {
            // Arrange
            var context = CriarHttpContext(autenticado: true, claimType: "Permissao", claimValue: "Ler");

            // Act
            var resultado = CustomAuthorization.ValidarClaimsUsuario(context, "Permissao", "Escrever");

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact(DisplayName = "ValidarClaimsUsuario autenticado com claim contendo valor parcial deve retornar true")]
        [Trait("Categoria", "WebApi.Core - Identidade - CustomAuthorization")]
        public void ValidarClaimsUsuario_ClaimContemValor_DeveRetornarTrue()
        {
            // Arrange
            var context = CriarHttpContext(autenticado: true, claimType: "Permissao", claimValue: "Ler,Escrever");

            // Act
            var resultado = CustomAuthorization.ValidarClaimsUsuario(context, "Permissao", "Ler");

            // Assert
            resultado.Should().BeTrue();
        }

        #endregion

        #region RequisitoClaimFilter

        [Fact(DisplayName = "RequisitoClaimFilter não autenticado deve retornar 401")]
        [Trait("Categoria", "WebApi.Core - Identidade - RequisitoClaimFilter")]
        public void OnAuthorization_NaoAutenticado_DeveRetornar401()
        {
            // Arrange
            var claim = new Claim("Permissao", "Ler");
            var filter = new RequisitoClaimFilter(claim);
            var context = CriarAuthorizationFilterContext(autenticado: false);

            // Act
            filter.OnAuthorization(context);

            // Assert
            var statusResult = context.Result as StatusCodeResult;
            statusResult.Should().NotBeNull();
            statusResult!.StatusCode.Should().Be(401);
        }

        [Fact(DisplayName = "RequisitoClaimFilter autenticado sem claim deve retornar 403")]
        [Trait("Categoria", "WebApi.Core - Identidade - RequisitoClaimFilter")]
        public void OnAuthorization_AutenticadoSemClaim_DeveRetornar403()
        {
            // Arrange
            var claim = new Claim("Permissao", "Escrever");
            var filter = new RequisitoClaimFilter(claim);
            var context = CriarAuthorizationFilterContext(autenticado: true, claimType: "Permissao", claimValue: "Ler");

            // Act
            filter.OnAuthorization(context);

            // Assert
            var statusResult = context.Result as StatusCodeResult;
            statusResult.Should().NotBeNull();
            statusResult!.StatusCode.Should().Be(403);
        }

        [Fact(DisplayName = "RequisitoClaimFilter autenticado com claim correta não deve definir Result")]
        [Trait("Categoria", "WebApi.Core - Identidade - RequisitoClaimFilter")]
        public void OnAuthorization_AutenticadoComClaim_NaoDeveDefinirResult()
        {
            // Arrange
            var claim = new Claim("Permissao", "Ler");
            var filter = new RequisitoClaimFilter(claim);
            var context = CriarAuthorizationFilterContext(autenticado: true, claimType: "Permissao", claimValue: "Ler");

            // Act
            filter.OnAuthorization(context);

            // Assert
            context.Result.Should().BeNull();
        }

        #endregion

        #region ClaimsAuthorizeAttribute

        [Fact(DisplayName = "ClaimsAuthorizeAttribute deve criar instância com Arguments")]
        [Trait("Categoria", "WebApi.Core - Identidade - ClaimsAuthorizeAttribute")]
        public void ClaimsAuthorizeAttribute_DeveCriarComArguments()
        {
            // Act
            var attribute = new ClaimsAuthorizeAttribute("Permissao", "Ler");

            // Assert
            attribute.Arguments.Should().NotBeNull();
            attribute.Arguments.Should().HaveCount(1);
            var claimArg = attribute.Arguments![0] as Claim;
            claimArg.Should().NotBeNull();
            claimArg!.Type.Should().Be("Permissao");
            claimArg.Value.Should().Be("Ler");
        }

        #endregion

        private static HttpContext CriarHttpContext(bool autenticado, string? claimType = null, string? claimValue = null)
        {
            var claims = new List<Claim>();
            if (claimType != null && claimValue != null)
                claims.Add(new Claim(claimType, claimValue));

            var identity = autenticado
                ? new ClaimsIdentity(claims, "Test")
                : new ClaimsIdentity(claims);

            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(identity)
            };

            return httpContext;
        }

        private static AuthorizationFilterContext CriarAuthorizationFilterContext(bool autenticado, string? claimType = null, string? claimValue = null)
        {
            var httpContext = CriarHttpContext(autenticado, claimType, claimValue);

            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            return new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
        }
    }
}

using PlataformaEducacao.GestaoIdentidade.Api.Models;

namespace PlataformaEducacao.GestaoIdentidade.Api.Tests.Models
{
    public class UsuarioViewModelsTests
    {
        [Fact(DisplayName = "UsuarioToken deve inicializar propriedades com valores padrões")]
        [Trait("Categoria", "Gestão Identidade - Models - UsuarioToken")]
        public void UsuarioToken_PropriedadesPadrao()
        {
            // Arrange & Act
            var token = new UsuarioToken();

            // Assert
            Assert.Equal(string.Empty, token.Id);
            Assert.Equal(string.Empty, token.Email);
            Assert.NotNull(token.Claims);
            Assert.Empty(token.Claims);
        }

        [Fact(DisplayName = "UsuarioClaim deve inicializar propriedades com valores padrões")]
        [Trait("Categoria", "Gestão Identidade - Models - UsuarioClaim")]
        public void UsuarioClaim_PropriedadesPadrao()
        {
            // Arrange & Act
            var claim = new UsuarioClaim();

            // Assert
            Assert.Equal(string.Empty, claim.Type);
            Assert.Equal(string.Empty, claim.Value);
        }

        [Fact(DisplayName = "UsuarioRespostaLogin deve inicializar propriedades com valores padrões")]
        [Trait("Categoria", "Gestão Identidade - Models - UsuarioRespostaLogin")]
        public void UsuarioRespostaLogin_PropriedadesPadrao()
        {
            // Arrange & Act
            var resposta = new UsuarioRespostaLogin();

            // Assert
            Assert.Equal(string.Empty, resposta.AccessToken);
            Assert.Equal(Guid.Empty, resposta.RefreshToken);
            Assert.Equal(0d, resposta.ExpiresIn);
            Assert.NotNull(resposta.UsuarioToken);
            Assert.IsType<UsuarioToken>(resposta.UsuarioToken);
        }

        [Fact(DisplayName = "UsuarioRefreshToken deve inicializar RefreshToken com string vazia")]
        [Trait("Categoria", "Gestão Identidade - Models - UsuarioRefreshToken")]
        public void UsuarioRefreshToken_PropriedadesPadrao()
        {
            // Arrange & Act
            var refresh = new UsuarioRefreshToken();

            // Assert
            Assert.Equal(string.Empty, refresh.RefreshToken);
        }
    }
}

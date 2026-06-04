using PlataformaEducacao.GestaoIdentidade.Api.Models;

namespace PlataformaEducacao.GestaoIdentidade.Api.Tests.Models
{
    public class UsuarioRespostaLoginTest
    {
        [Fact(DisplayName = "UsuarioRespostaLogin deve atribuir propriedades corretamente")]
        [Trait("Categoria", "Gestão Identidade - Models - UsuarioRespostaLogin")]
        public void UsuarioRespostaLogin_DeveAtribuirPropriedades()
        {
            // Arrange & Act
            var resposta = new UsuarioRespostaLogin
            {
                AccessToken = "token123",
                ExpiresIn = 3600,
                UsuarioToken = new UsuarioToken
                {
                    Id = "id-user",
                    Email = "user@teste.com",
                    Claims = new List<UsuarioClaim>
                    {
                        new UsuarioClaim { Type = "role", Value = "ALUNO" }
                    }
                }
            };

            // Assert
            Assert.Equal("token123", resposta.AccessToken);
            Assert.Equal(3600, resposta.ExpiresIn);
            Assert.Equal("id-user", resposta.UsuarioToken.Id);
            Assert.Equal("user@teste.com", resposta.UsuarioToken.Email);
            Assert.Single(resposta.UsuarioToken.Claims);
        }

        [Fact(DisplayName = "UsuarioToken deve ter valores padrão")]
        [Trait("Categoria", "Gestão Identidade - Models - UsuarioToken")]
        public void UsuarioToken_DeveTerValoresPadrao()
        {
            // Arrange & Act
            var token = new UsuarioToken();

            // Assert
            Assert.Equal(string.Empty, token.Id);
            Assert.Equal(string.Empty, token.Email);
            Assert.NotNull(token.Claims);
            Assert.Empty(token.Claims);
        }

        [Fact(DisplayName = "UsuarioClaim deve atribuir propriedades")]
        [Trait("Categoria", "Gestão Identidade - Models - UsuarioClaim")]
        public void UsuarioClaim_DeveAtribuirPropriedades()
        {
            // Arrange & Act
            var claim = new UsuarioClaim
            {
                Type = "role",
                Value = "ADMIN"
            };

            // Assert
            Assert.Equal("role", claim.Type);
            Assert.Equal("ADMIN", claim.Value);
        }
    }
}

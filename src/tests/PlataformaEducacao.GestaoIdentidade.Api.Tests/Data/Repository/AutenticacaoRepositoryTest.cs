using Microsoft.EntityFrameworkCore;
using PlataformaEducacao.GestaoIdentidade.Api.Data;
using PlataformaEducacao.GestaoIdentidade.Api.Data.Repository;
using PlataformaEducacao.GestaoIdentidade.Api.Models;

namespace PlataformaEducacao.GestaoIdentidade.Api.Tests.Data.Repository
{
    public class AutenticacaoRepositoryTest
    {
        [Fact(DisplayName = "AdicionarRefreshToken Deve Remover Tokens Existentes E Adicionar Novo")]
        [Trait("Categoria", "Gestão Identidade - Data - AutenticacaoRepository")]
        public async Task AdicionarRefreshToken_Deve_RemoverTokensExistentes_E_AdicionarNovo()
        {
            // Arrange
            var opcoes = new DbContextOptionsBuilder<GestaoIdentidadeContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            await using var contexto = new GestaoIdentidadeContext(opcoes);
            var tokenExistente = new RefreshToken { UserName = "usuario.test", ExpirationDate = DateTime.Now.AddHours(1) };
            await contexto.RefreshTokens.AddAsync(tokenExistente);
            await contexto.SaveChangesAsync();

            var repositorio = new AutenticacaoRepository(contexto);
            var tokenNovo = new RefreshToken { UserName = "usuario.test", Token = Guid.NewGuid(), ExpirationDate = DateTime.Now.AddHours(2) };

            // Act
            await repositorio.AdicionarRefreshToken(tokenNovo);

            // Assert
            var tokens = contexto.RefreshTokens.AsNoTracking().Where(t => t.UserName == "usuario.test").ToList();
            Assert.Single(tokens);
            Assert.Equal(tokenNovo.Token, tokens[0].Token);
        }

        [Fact(DisplayName = "ObterRefreshToken Deve Retornar Token Quando Valid")]
        [Trait("Categoria", "Gestão Identidade - Data - AutenticacaoRepository")]
        public async Task ObterRefreshToken_Deve_Retornar_Token_Quando_Valido()
        {
            // Arrange
            var opcoes = new DbContextOptionsBuilder<GestaoIdentidadeContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            await using var contexto = new GestaoIdentidadeContext(opcoes);
            var tokenValido = new RefreshToken { UserName = "usuario2", Token = Guid.NewGuid(), ExpirationDate = DateTime.Now.AddHours(1) };
            await contexto.RefreshTokens.AddAsync(tokenValido);
            await contexto.SaveChangesAsync();

            var repositorio = new AutenticacaoRepository(contexto);

            // Act
            var obtido = await repositorio.ObterRefreshToken(tokenValido.Token);

            // Assert
            Assert.NotNull(obtido);
            Assert.Equal(tokenValido.Token, obtido!.Token);
        }

        [Fact(DisplayName = "ObterRefreshToken Deve Retornar Nulo Quando Expirado")]
        [Trait("Categoria", "Gestão Identidade - Data - AutenticacaoRepository")]
        public async Task ObterRefreshToken_Deve_Retornar_Nulo_Quando_Expirado()
        {
            // Arrange
            var opcoes = new DbContextOptionsBuilder<GestaoIdentidadeContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            await using var contexto = new GestaoIdentidadeContext(opcoes);
            var tokenExpirado = new RefreshToken { UserName = "usuario3", Token = Guid.NewGuid(), ExpirationDate = DateTime.Now.AddHours(-1) };
            await contexto.RefreshTokens.AddAsync(tokenExpirado);
            await contexto.SaveChangesAsync();

            var repositorio = new AutenticacaoRepository(contexto);

            // Act
            var obtido = await repositorio.ObterRefreshToken(tokenExpirado.Token);

            // Assert
            Assert.Null(obtido);
        }

        [Fact(DisplayName = "ObterRefreshToken Deve Retornar Nulo Quando Não Encontrado")]
        [Trait("Categoria", "Gestão Identidade - Data - AutenticacaoRepository")]
        public async Task ObterRefreshToken_Deve_Retornar_Nulo_Quando_Nao_Encontrado()
        {
            // Arrange
            var opcoes = new DbContextOptionsBuilder<GestaoIdentidadeContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            await using var contexto = new GestaoIdentidadeContext(opcoes);
            var repositorio = new AutenticacaoRepository(contexto);

            // Act
            var obtido = await repositorio.ObterRefreshToken(Guid.NewGuid());

            // Assert
            Assert.Null(obtido);
        }
    }
}

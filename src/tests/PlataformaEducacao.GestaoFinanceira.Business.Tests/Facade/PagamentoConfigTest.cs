using PlataformaEducacao.GestaoFinanceira.Business.Facade;

namespace PlataformaEducacao.GestaoFinanceira.Business.Tests.Facade
{
    public class PagamentoConfigTest
    {
        [Fact(DisplayName = "PagamentoConfig deve atribuir propriedades")]
        [Trait("Categoria", "Gestão Financeira - Facade - PagamentoConfig")]
        public void PagamentoConfig_DeveAtribuirPropriedades()
        {
            // Act
            var config = new PagamentoConfig
            {
                DefaultApiKey = "api-key-123",
                DefaultEncryptionKey = "enc-key-456"
            };

            // Assert
            Assert.Equal("api-key-123", config.DefaultApiKey);
            Assert.Equal("enc-key-456", config.DefaultEncryptionKey);
        }
    }
}

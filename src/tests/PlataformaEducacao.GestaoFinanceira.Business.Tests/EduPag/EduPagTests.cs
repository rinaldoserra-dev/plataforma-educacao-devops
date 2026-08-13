using PlataformaEducacao.GestaoFinanceira.EduPag;

namespace PlataformaEducacao.GestaoFinanceira.Business.Tests.EduPag
{
    public class EduPagServiceTest
    {
        [Fact(DisplayName = "EduPagService deve atribuir ApiKey e EncryptionKey")]
        [Trait("Categoria", "Gestão Financeira - EduPag - EduPagService")]
        public void EduPagService_DeveAtribuirChaves()
        {
            // Act
            var service = new EduPagService("minha-api-key", "minha-enc-key");

            // Assert
            Assert.Equal("minha-api-key", service.ApiKey);
            Assert.Equal("minha-enc-key", service.EncryptionKey);
        }
    }
}

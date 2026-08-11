using PlataformaEducacao.GestaoFinanceira.EduPag;

namespace PlataformaEducacao.GestaoFinanceira.Business.Tests.EduPag
{

    public class CardHashTest
    {
        [Fact(DisplayName = "CardHash.Generate deve retornar string não vazia")]
        [Trait("Categoria", "Gestão Financeira - EduPag - CardHash")]
        public void Generate_DeveRetornarHashNaoVazio()
        {
            // Arrange
            var svc = new EduPagService("0123456789abcdef0123456789abcdef", "abcdefghijklmnop");
            var cardHash = new CardHash(svc)
            {
                CardHolderName = "Fulano",
                CardNumber = "4111111111111111",
                CardExpirationDate = "12/2030",
                CardCvv = "123"
            };

            // Act
            var hash = cardHash.Generate();

            // Assert
            Assert.NotNull(hash);
            Assert.NotEmpty(hash);
        }
    }
}

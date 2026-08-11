using PlataformaEducacao.GestaoFinanceira.EduPag;

namespace PlataformaEducacao.GestaoFinanceira.Business.Tests.EduPag
{
    public class PaymentMethodTest
    {
        [Fact(DisplayName = "PaymentMethod deve conter valores esperados")]
        [Trait("Categoria", "Gestão Financeira - EduPag - PaymentMethod")]
        public void PaymentMethod_ValoresEsperados()
        {
            Assert.Equal(1, (int)PaymentMethod.CreditCard);
            Assert.Equal(2, (int)PaymentMethod.Billet);
        }
    }
}

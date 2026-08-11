using PlataformaEducacao.GestaoFinanceira.EduPag;

namespace PlataformaEducacao.GestaoFinanceira.Business.Tests.EduPag
{

    public class TransactionStatusTest
    {
        [Fact(DisplayName = "TransactionStatus deve conter valores esperados")]
        [Trait("Categoria", "Gestão Financeira - EduPag - TransactionStatus")]
        public void TransactionStatus_ValoresEsperados()
        {
            Assert.Equal(1, (int)TransactionStatus.Authorized);
            Assert.Equal(2, (int)TransactionStatus.Paid);
            Assert.Equal(3, (int)TransactionStatus.Refused);
            Assert.Equal(4, (int)TransactionStatus.Chargedback);
            Assert.Equal(5, (int)TransactionStatus.Cancelled);
        }
    }
}

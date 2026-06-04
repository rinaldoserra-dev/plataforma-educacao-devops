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

    public class TransactionTest
    {
        private static EduPagService CriarEduPagService()
        {
            return new EduPagService("0123456789abcdef0123456789abcdef", "abcdefghijklmnop");
        }

        [Fact(DisplayName = "AuthorizeCardTransaction deve retornar Authorized ou Refused")]
        [Trait("Categoria", "Gestão Financeira - EduPag - Transaction")]
        public async Task AuthorizeCardTransaction_DeveRetornarTransacao()
        {
            // Arrange
            var svc = CriarEduPagService();
            var transaction = new Transaction(svc)
            {
                CardNumber = "4111111111111111",
                CardHolderName = "Fulano",
                CardExpirationDate = "12/2030",
                CardCvv = "123",
                PaymentMethod = PaymentMethod.CreditCard,
                Amount = 100m
            };

            // Act
            var result = await transaction.AuthorizeCardTransaction();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Status == TransactionStatus.Authorized || result.Status == TransactionStatus.Refused);
        }

        [Fact(DisplayName = "CaptureCardTransaction deve retornar status Paid")]
        [Trait("Categoria", "Gestão Financeira - EduPag - Transaction")]
        public async Task CaptureCardTransaction_DeveRetornarPaid()
        {
            // Arrange
            var svc = CriarEduPagService();
            var transaction = new Transaction(svc)
            {
                Amount = 200m,
                CardBrand = "MasterCard",
                Tid = "TID123",
                Nsu = "NSU456"
            };

            // Act
            var result = await transaction.CaptureCardTransaction();

            // Assert
            Assert.Equal(TransactionStatus.Paid, result.Status);
            Assert.Equal(200m, result.Amount);
            Assert.Equal("MasterCard", result.CardBrand);
            Assert.Equal("TID123", result.Tid);
            Assert.Equal("NSU456", result.Nsu);
        }

        [Fact(DisplayName = "CancelAuthorization deve retornar status Cancelled")]
        [Trait("Categoria", "Gestão Financeira - EduPag - Transaction")]
        public async Task CancelAuthorization_DeveRetornarCancelled()
        {
            // Arrange
            var svc = CriarEduPagService();
            var transaction = new Transaction(svc)
            {
                Amount = 150m,
                CardBrand = "Visa",
                Tid = "TID789",
                Nsu = "NSU012"
            };

            // Act
            var result = await transaction.CancelAuthorization();

            // Assert
            Assert.Equal(TransactionStatus.Cancelled, result.Status);
            Assert.Equal(150m, result.Amount);
            Assert.Equal("Visa", result.CardBrand);
            Assert.Equal(string.Empty, result.AuthorizationCode);
        }

        [Fact(DisplayName = "Transaction deve atribuir propriedades")]
        [Trait("Categoria", "Gestão Financeira - EduPag - Transaction")]
        public void Transaction_DeveAtribuirPropriedades()
        {
            // Arrange & Act
            var svc = CriarEduPagService();
            var t = new Transaction(svc)
            {
                SubscriptionId = 1,
                Status = TransactionStatus.Authorized,
                AuthorizationAmount = 100,
                PaidAmount = 100,
                RefundedAmount = 0,
                CardHash = "hash",
                CardNumber = "4111111111111111",
                CardExpirationDate = "12/30",
                StatusReason = "ok",
                AcquirerResponseCode = "00",
                AcquirerName = "Acquirer",
                AuthorizationCode = "AUTH",
                SoftDescriptor = "Desc",
                RefuseReason = "",
                Tid = "TID",
                Nsu = "NSU",
                Amount = 100m,
                Installments = 1,
                Cost = 3m,
                CardHolderName = "Fulano",
                CardCvv = "123",
                CardLastDigits = "1111",
                CardFirstDigits = "4111",
                CardBrand = "Visa",
                CardEmvResponse = "",
                PostbackUrl = "http://callback",
                PaymentMethod = PaymentMethod.CreditCard,
                AntifraudScore = 95.5f,
                BilletUrl = "",
                BilletInstructions = "",
                BilletExpirationDate = null,
                BilletBarcode = "",
                Referer = "http://site",
                IP = "127.0.0.1",
                ShouldCapture = true,
                Async = false,
                LocalTime = "12:00",
                TransactionDate = DateTime.UtcNow
            };

            // Assert
            Assert.Equal(1, t.SubscriptionId);
            Assert.Equal(TransactionStatus.Authorized, t.Status);
            Assert.Equal(100, t.AuthorizationAmount);
            Assert.Equal(100, t.PaidAmount);
            Assert.Equal("hash", t.CardHash);
            Assert.Equal("4111111111111111", t.CardNumber);
            Assert.Equal("AUTH", t.AuthorizationCode);
            Assert.Equal("TID", t.Tid);
            Assert.Equal("NSU", t.Nsu);
            Assert.Equal(100m, t.Amount);
            Assert.Equal(1, t.Installments);
            Assert.Equal(3m, t.Cost);
            Assert.Equal("Fulano", t.CardHolderName);
            Assert.Equal("123", t.CardCvv);
            Assert.Equal("Visa", t.CardBrand);
            Assert.Equal(PaymentMethod.CreditCard, t.PaymentMethod);
            Assert.True(t.ShouldCapture);
            Assert.False(t.Async);
        }
    }

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

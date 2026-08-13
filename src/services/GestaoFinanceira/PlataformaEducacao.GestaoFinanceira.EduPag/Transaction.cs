namespace PlataformaEducacao.GestaoFinanceira.EduPag
{
    public class Transaction
    {
        private readonly EduPagService _eduPagService = null!;

        public Transaction(EduPagService eduPagService)
        {
            _eduPagService = eduPagService;
        }

        public int SubscriptionId { get; set; }

        protected Transaction()
        {
        }

        public TransactionStatus Status { get; set; }

        public int AuthorizationAmount { get; set; }

        public int PaidAmount { get; set; }

        public int RefundedAmount { get; set; }

        public string CardHash { get; set; } = string.Empty;

        public string CardNumber { get; set; } = string.Empty;

        public string CardExpirationDate { get; set; } = string.Empty;

        public string StatusReason { get; set; } = string.Empty;

        public string AcquirerResponseCode { get; set; } = string.Empty;

        public string AcquirerName { get; set; } = string.Empty;

        public string AuthorizationCode { get; set; } = string.Empty;

        public string SoftDescriptor { get; set; } = string.Empty;

        public string RefuseReason { get; set; } = string.Empty;

        public string Tid { get; set; } = string.Empty;

        public string Nsu { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public int? Installments { get; set; }

        public decimal Cost { get; set; }

        public string CardHolderName { get; set; } = string.Empty;

        public string CardCvv { get; set; } = string.Empty;

        public string CardLastDigits { get; set; } = string.Empty;

        public string CardFirstDigits { get; set; } = string.Empty;

        public string CardBrand { get; set; } = string.Empty;

        public string CardEmvResponse { get; set; } = string.Empty;

        public string PostbackUrl { get; set; } = string.Empty;

        public PaymentMethod PaymentMethod { get; set; }

        public float? AntifraudScore { get; set; }

        public string BilletUrl { get; set; } = string.Empty;

        public string BilletInstructions { get; set; } = string.Empty;

        public DateTime? BilletExpirationDate { get; set; }

        public string BilletBarcode { get; set; } = string.Empty;

        public string Referer { get; set; } = string.Empty;

        public string IP { get; set; } = string.Empty;

        public bool? ShouldCapture { get; set; }

        public bool? Async { get; set; }

        public string LocalTime { get; set; } = string.Empty;

        public DateTime TransactionDate { get; set; }

        public Task<Transaction> AuthorizeCardTransaction()
        {
            var success = new Random().Next(2) == 0;
            Transaction transaction;

            if (success)
            {
                transaction = new Transaction
                {
                    AuthorizationCode = GetGenericCode(),
                    CardBrand = "MasterCard",
                    TransactionDate = DateTime.Now,
                    Cost = Amount * 0.03M,
                    Amount = Amount,
                    Status = TransactionStatus.Authorized,
                    Tid = GetGenericCode(),
                    Nsu = GetGenericCode()
                };

                return Task.FromResult(transaction);
            }

            transaction = new Transaction
            {
                AuthorizationCode = string.Empty,
                CardBrand = string.Empty,
                TransactionDate = DateTime.Now,
                Cost = 0,
                Amount = 0,
                Status = TransactionStatus.Refused,
                Tid = string.Empty,
                Nsu = string.Empty
            };

            return Task.FromResult(transaction);
        }

        public Task<Transaction> CaptureCardTransaction()
        {
            var transaction = new Transaction
            {
                AuthorizationCode = GetGenericCode(),
                CardBrand = CardBrand,
                TransactionDate = DateTime.Now,
                Cost = 0,
                Amount = Amount,
                Status = TransactionStatus.Paid,
                Tid = Tid,
                Nsu = Nsu
            };

            return Task.FromResult(transaction);
        }

        public Task<Transaction> CancelAuthorization()
        {
            var transaction = new Transaction
            {
                AuthorizationCode = string.Empty,
                CardBrand = CardBrand,
                TransactionDate = DateTime.Now,
                Cost = 0,
                Amount = Amount,
                Status = TransactionStatus.Cancelled,
                Tid = Tid,
                Nsu = Nsu
            };

            return Task.FromResult(transaction);
        }

        protected string Endpoint { get; set; } = string.Empty;

        private static string GetGenericCode()
        {
            return new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 10)
                .Select(s => s[new Random().Next(s.Length)]).ToArray());
        }
    }
}

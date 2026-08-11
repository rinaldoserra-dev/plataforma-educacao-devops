using System.Security.Cryptography;
using System.Text;

namespace PlataformaEducacao.GestaoFinanceira.EduPag
{
    public class CardHash
    {
        private readonly EduPagService _eduPagService;

        public CardHash(EduPagService eduPagService)
        {
            _eduPagService = eduPagService;
        }

        public string CardHolderName { get; set; } = string.Empty;

        public string CardNumber { get; set; } = string.Empty;

        public string CardExpirationDate { get; set; } = string.Empty;

        public string CardCvv { get; set; } = string.Empty;

        public string Generate()
        {
            using var aesAlg = Aes.Create();

            aesAlg.IV = Encoding.Default.GetBytes(_eduPagService.EncryptionKey);
            aesAlg.Key = Encoding.Default.GetBytes(_eduPagService.ApiKey);

            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using var msEncrypt = new MemoryStream();
            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(CardHolderName + CardNumber + CardExpirationDate + CardCvv);
            }

            return Encoding.ASCII.GetString(msEncrypt.ToArray());
        }
    }
}

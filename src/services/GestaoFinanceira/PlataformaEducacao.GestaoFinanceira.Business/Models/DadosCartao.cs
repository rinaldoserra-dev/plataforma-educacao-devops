using PlataformaEducacao.Core;

namespace PlataformaEducacao.GestaoFinanceira.Business.Models
{
    public class DadosCartao
    {
        public DadosCartao(string nomeCartao, string numeroCartao, string expiracaoCartao, string cvvCartao)
        {
            NomeCartao = nomeCartao;
            NumeroCartao = numeroCartao;
            ExpiracaoCartao = expiracaoCartao;
            CvvCartao = cvvCartao;

            Validar();
        }

        public string NomeCartao { get; private set; } = null!;
        public string NumeroCartao { get; private set; } = null!;
        public string ExpiracaoCartao { get; private set; } = null!;
        public string CvvCartao { get; private set; } = null!;

        protected void Validar()
        {
            Validacoes.ValidarSeVazio(NomeCartao, "O nome do cartão é obrigatório.");
            Validacoes.ValidarSeVazio(NumeroCartao, "O número do cartão é obrigatório.");
            Validacoes.ValidarSeVazio(ExpiracaoCartao, "A expiração cartão é obrigatória.");
            Validacoes.ValidarSeVazio(CvvCartao, "O código de verificação do cartão é obrigatório.");
        }
    }
}

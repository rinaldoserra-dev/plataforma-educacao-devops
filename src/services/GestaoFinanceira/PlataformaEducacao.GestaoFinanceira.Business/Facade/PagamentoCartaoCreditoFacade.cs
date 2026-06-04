using Microsoft.Extensions.Options;
using PlataformaEducacao.GestaoFinanceira.Business.Models;
using PlataformaEducacao.GestaoFinanceira.EduPag;
using Transaction = PlataformaEducacao.GestaoFinanceira.EduPag.Transaction;
using TransactionStatus = PlataformaEducacao.GestaoFinanceira.EduPag.TransactionStatus;

namespace PlataformaEducacao.GestaoFinanceira.Business.Facade
{
    public class PagamentoCartaoCreditoFacade : IPagamentoCartaoCreditoFacade
    {
        private readonly PagamentoConfig _pagamentoConfig;

        public PagamentoCartaoCreditoFacade(IOptions<PagamentoConfig> pagamentoConfig)
        {
            _pagamentoConfig = pagamentoConfig.Value;
        }


        public async Task<Transacao> AutorizarPagamento(Pagamento pagamento)
        {
            var eduPagSvc = new EduPagService(_pagamentoConfig.DefaultApiKey,
               _pagamentoConfig.DefaultEncryptionKey);

            var cardHashGen = new CardHash(eduPagSvc)
            {
                CardNumber = pagamento.DadosCartao.NumeroCartao,
                CardHolderName = pagamento.DadosCartao.NomeCartao,
                CardExpirationDate = pagamento.DadosCartao.ExpiracaoCartao,
                CardCvv = pagamento.DadosCartao.CvvCartao
            };
            var cardHash = cardHashGen.Generate();

            var transacao = new Transaction(eduPagSvc)
            {
                CardHash = cardHash,
                CardNumber = pagamento.DadosCartao.NumeroCartao,
                CardHolderName = pagamento.DadosCartao.NomeCartao,
                CardExpirationDate = pagamento.DadosCartao.ExpiracaoCartao,
                CardCvv = pagamento.DadosCartao.CvvCartao,
                PaymentMethod = PaymentMethod.CreditCard,
                Amount = pagamento.Valor
            };

            return ParaTransacao(await transacao.AuthorizeCardTransaction());
        }


        public async Task<Transacao> CapturarPagamento(Transacao transacao)
        {
            var eduPagSvc = new EduPagService(_pagamentoConfig.DefaultApiKey,
                _pagamentoConfig.DefaultEncryptionKey);

            var transaction = ParaTransaction(transacao, eduPagSvc);

            return ParaTransacao(await transaction.CaptureCardTransaction());
        }


        public async Task<Transacao> CancelarAutorizacao(Transacao transacao)
        {
            var eduPagSvc = new EduPagService(_pagamentoConfig.DefaultApiKey,
                   _pagamentoConfig.DefaultEncryptionKey);

            var transaction = ParaTransaction(transacao, eduPagSvc);

            return ParaTransacao(await transaction.CancelAuthorization());
        }

        public static Transacao ParaTransacao(Transaction transaction)
        {
            return new Transacao
            {
                Id = Guid.NewGuid(),
                Status = (StatusTransacao)transaction.Status,
                ValorTotal = transaction.Amount,
                BandeiraCartao = transaction.CardBrand,
                CodigoAutorizacao = transaction.AuthorizationCode,
                CustoTransacao = transaction.Cost,
                DataTransacao = transaction.TransactionDate,
                NSU = transaction.Nsu,
                TID = transaction.Tid
            };
        }

        public static Transaction ParaTransaction(Transacao transacao, EduPagService eduPagService)
        {
            return new Transaction(eduPagService)
            {
                Status = (TransactionStatus)transacao.Status,
                Amount = transacao.ValorTotal,
                CardBrand = transacao.BandeiraCartao,
                AuthorizationCode = transacao.CodigoAutorizacao,
                Cost = transacao.CustoTransacao,
                Nsu = transacao.NSU,
                Tid = transacao.TID
            };
        }
    }
}

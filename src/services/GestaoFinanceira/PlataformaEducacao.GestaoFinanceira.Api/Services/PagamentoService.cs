using FluentValidation.Results;
using PlataformaEducacao.Core.DomainObjects;
using PlataformaEducacao.Core.Messages.Integration;
using PlataformaEducacao.GestaoFinanceira.Api.Models.Response;
using PlataformaEducacao.GestaoFinanceira.Business.Facade;
using PlataformaEducacao.GestaoFinanceira.Business.Models;
using PlataformaEducacao.MessageBus;

namespace PlataformaEducacao.GestaoFinanceira.Api.Services
{
    public class PagamentoService : IPagamentoService
    {
        private readonly IPagamentoCartaoCreditoFacade _pagamentoFacade;
        private readonly IPagamentoRepository _pagamentoRepository;
        private readonly IMessageBus _bus;

        public PagamentoService(IPagamentoCartaoCreditoFacade pagamentoFacade,
                                IPagamentoRepository pagamentoRepository,
                                IMessageBus bus)
        {
            _pagamentoFacade = pagamentoFacade;
            _pagamentoRepository = pagamentoRepository;
            _bus = bus;
        }

        public async Task<ResponseMessage> AutorizarPagamento(Pagamento pagamento, CancellationToken cancellationToken)
        {
            var transacao = await _pagamentoFacade.AutorizarPagamento(pagamento);
            var validationResult = new ValidationResult();

            if (transacao.Status != StatusTransacao.Autorizado)
            {

                validationResult.Errors.Add(new ValidationFailure("Pagamento",
                        "Pagamento recusado, entre em contato com a sua operadora de cartão"));

                await _bus.PublishAsync(new MatriculaPagamentoRecusadoIntegrationEvent(pagamento.MatriculaId));

                return new ResponseMessage(validationResult);
            }

            pagamento.AdicionarTransacao(transacao);
            _pagamentoRepository.AdicionarPagamento(pagamento);

            if (!await _pagamentoRepository.UnitOfWork.Commit())
            {
                validationResult.Errors.Add(new ValidationFailure("Pagamento", "Houve um erro ao realizar o pagamento."));

                // Cancelar pagamento no gateway
                await CancelarPagamento(pagamento.MatriculaId);

                return new ResponseMessage(validationResult);
            }

            try
            {
                await _bus.PublishAsync(new MatriculaPagamentoRealizadoIntegrationEvent(pagamento.MatriculaId));
            }
            catch (Exception)
            {
                // log
                validationResult.Errors.Add(new ValidationFailure("Pagamento", "Pagamento autorizado, mas houve falha ao notificar os demais serviços."));

                // Cancelar pagamento no gateway
                await CancelarPagamento(pagamento.MatriculaId);

                return new ResponseMessage(validationResult);
            }

            return new ResponseMessage(validationResult);
        }

        public async Task<ResponseMessage> CapturarPagamento(Guid matriculaId)
        {
            var transacoes = await _pagamentoRepository.ObterTransacoesPorMatriculaId(matriculaId);
            var transacaoAutorizada = transacoes?.FirstOrDefault(t => t.Status == StatusTransacao.Autorizado);
            var validationResult = new ValidationResult();

            if (transacaoAutorizada == null) throw new DomainException($"Transação não encontrada para a matricula {matriculaId}");

            var transacao = await _pagamentoFacade.CapturarPagamento(transacaoAutorizada);

            if (transacao.Status != StatusTransacao.Pago)
            {
                validationResult.Errors.Add(new ValidationFailure("Pagamento",
                    $"Não foi possível capturar o pagamento da matricula {matriculaId}"));

                return new ResponseMessage(validationResult);
            }

            transacao.PagamentoId = transacaoAutorizada.PagamentoId;
            _pagamentoRepository.AdicionarTransacao(transacao);

            if (!await _pagamentoRepository.UnitOfWork.Commit())
            {
                validationResult.Errors.Add(new ValidationFailure("Pagamento",
                    $"Não foi possível persistir a captura do pagamento da matricula {matriculaId}"));

                return new ResponseMessage(validationResult);
            }

            return new ResponseMessage(validationResult);
        }

        public async Task<ResponseMessage> CancelarPagamento(Guid matriculaId)
        {
            var transacoes = await _pagamentoRepository.ObterTransacoesPorMatriculaId(matriculaId);
            var transacaoAutorizada = transacoes?.FirstOrDefault(t => t.Status == StatusTransacao.Autorizado);
            var validationResult = new ValidationResult();

            if (transacaoAutorizada == null) throw new DomainException($"Transação não encontrada para a matricula {matriculaId}");

            var transacao = await _pagamentoFacade.CancelarAutorizacao(transacaoAutorizada);

            if (transacao.Status != StatusTransacao.Cancelado)
            {
                validationResult.Errors.Add(new ValidationFailure("Pagamento",
                    $"Não foi possível cancelar o pagamento da matricula {matriculaId}"));

                return new ResponseMessage(validationResult);
            }

            transacao.PagamentoId = transacaoAutorizada.PagamentoId;
            _pagamentoRepository.AdicionarTransacao(transacao);

            if (!await _pagamentoRepository.UnitOfWork.Commit())
            {
                validationResult.Errors.Add(new ValidationFailure("Pagamento",
                    $"Não foi possível persistir o cancelamento do pagamento da matricula {matriculaId}"));

                return new ResponseMessage(validationResult);
            }

            return new ResponseMessage(validationResult);
        }

        public async Task<PagamentoStatusResponse?> ObterStatusPorMatricula(Guid matriculaId, Guid usuarioId, bool isAdm)
        {
            var pagamento = await _pagamentoRepository.ObterPagamentoPorMatriculaId(matriculaId, usuarioId, isAdm);

            if (pagamento == null)
                return null;

            var ultimaTransacao = pagamento.Transacoes
                .OrderByDescending(t => t.DataTransacao ?? DateTime.MinValue)
                .FirstOrDefault();

            return new PagamentoStatusResponse
            {
                MatriculaId = pagamento.MatriculaId,
                Status = ultimaTransacao?.Status.ToString() ?? "Sem transações"
            };
        }

    }
}

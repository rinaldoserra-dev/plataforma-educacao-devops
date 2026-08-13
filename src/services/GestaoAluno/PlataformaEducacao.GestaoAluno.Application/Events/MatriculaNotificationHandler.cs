using MediatR;
using PlataformaEducacao.Core.Mediator;
using PlataformaEducacao.GestaoAluno.Application.Commands.GerarCertificado;
using PlataformaEducacao.GestaoAluno.Domain.Events;

namespace PlataformaEducacao.GestaoAluno.Application.Events
{
    public class MatriculaNotificationHandler :
        INotificationHandler<CursoFinalizadoEvent>,
        INotificationHandler<MatriculaAtivadaEvent>
    {
        private readonly IMediatorHandler _mediatorHandler;

        public MatriculaNotificationHandler(IMediatorHandler mediatorHandler)
        {
            _mediatorHandler = mediatorHandler;
        }

        public async Task Handle(CursoFinalizadoEvent notification, CancellationToken cancellationToken)
        {
            await _mediatorHandler.SendCommand(new GerarCertificadoCommand(notification.MatriculaId));
        }

        public Task Handle(MatriculaAtivadaEvent notification, CancellationToken cancellationToken)
        {
            // envio de email de boas vindas
            return Task.CompletedTask;
        }
    }
}

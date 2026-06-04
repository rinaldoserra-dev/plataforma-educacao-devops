using MediatR;
using PlataformaEducacao.Core.Mediator;
using PlataformaEducacao.GestaoAluno.Application.Commands.GerarCertificado;
using PlataformaEducacao.GestaoAluno.Domain.Events;

namespace PlataformaEducacao.GestaoAluno.Application.Events
{
    public class MatriculaEventHandler :
        INotificationHandler<CursoFinalizadoEvent>,
        INotificationHandler<MatriculaAtivadaEvent>
    {
        private readonly IMediatorHandler _mediatorHandler;

        public MatriculaEventHandler(IMediatorHandler mediatorHandler)
        {
            _mediatorHandler = mediatorHandler;
        }
        public async Task Handle(CursoFinalizadoEvent message, CancellationToken cancellationToken)
        {
            await _mediatorHandler.SendCommand(new GerarCertificadoCommand(message.MatriculaId));
        }
        public Task Handle(MatriculaAtivadaEvent notification, CancellationToken cancellationToken)
        {
            //envio de email de boas vindas
            return Task.CompletedTask;
        }
    }
}

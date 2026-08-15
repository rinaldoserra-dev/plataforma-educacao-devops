using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlataformaEducacao.Core.Mediator;
using PlataformaEducacao.Core.Messages.Integration;
using PlataformaEducacao.GestaoAluno.Application.Commands.AdicionarAluno;
using PlataformaEducacao.MessageBus;

namespace PlataformaEducacao.GestaoAluno.Application.Services
{
    public class RegistroAlunoIntegrationHandler : BackgroundService
    {
        private readonly IMessageBus _bus;
        private readonly IServiceProvider _serviceProvider;

        public RegistroAlunoIntegrationHandler(IMessageBus bus, IServiceProvider serviceProvider)
        {
            _bus = bus;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    SetResponder();
                    await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
                }
                catch (Exception) when (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }
        }

        private void SetResponder()
        {
            _bus.RespondAsync<UsuarioRegistradoIntegrationEvent, ResponseMessage>(async request =>
                await RegistrarCliente(request));

            _bus.AdvancedBus.Connected += OnConnect!;
        }

        private void OnConnect(object s, EventArgs e)
        {
            SetResponder();
        }

        private async Task<ResponseMessage> RegistrarCliente(UsuarioRegistradoIntegrationEvent message)
        {
            var clienteCommand = new AdicionarAlunoCommand(message.UsuarioId, message.Nome, message.Email);
            ValidationResult sucesso;

            using (var scope = _serviceProvider.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediatorHandler>();
                sucesso = await mediator.SendCommand(clienteCommand);
            }

            return new ResponseMessage(sucesso);
        }
    }
}

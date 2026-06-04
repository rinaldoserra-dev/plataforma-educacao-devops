namespace PlataformaEducacao.Core.Messages.Integration
{
    public class UsuarioRegistradoIntegrationEvent : IntegrationEvent
    {
        public UsuarioRegistradoIntegrationEvent(Guid usuarioId, string nome, string email)
        {
            AggregateId = usuarioId;
            UsuarioId = usuarioId;
            Nome = nome;
            Email = email;
        }

        public Guid UsuarioId { get; private set; }
        public string Nome { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
    }
}

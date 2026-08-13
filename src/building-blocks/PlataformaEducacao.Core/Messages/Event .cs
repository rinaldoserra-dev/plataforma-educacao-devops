using MediatR;

namespace PlataformaEducacao.Core.Messages
{
    public class Evento : Message, INotification
    {
        protected Evento()
        {
            Timestamp = DateTime.Now;
        }

        public DateTime Timestamp { get; private set; }
    }
}

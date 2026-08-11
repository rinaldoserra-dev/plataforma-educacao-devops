using PlataformaEducacao.Core.Messages;

namespace PlataformaEducacao.Core.DomainObjects
{
    public abstract class Entity
    {
        private List<Evento> _notificacoes;

        protected Entity()
        {
            Id = Guid.NewGuid();
            _notificacoes = [];
        }

        public Guid Id { get; set; }

        public static bool operator ==(
            Entity? a, Entity? b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public IReadOnlyCollection<Evento> Notificacoes => _notificacoes.AsReadOnly();

        public void AdicionarEvento(Evento evento)
        {
            _notificacoes ??= [];
            _notificacoes.Add(evento);
        }

        public void DefinirId(Guid id)
        {
            Id = id;
        }

        public void RemoverEvento(Evento eventItem)
        {
            _notificacoes?.Remove(eventItem);
        }

        public void LimparEventos()
        {
            _notificacoes?.Clear();
        }

        public override bool Equals(object? obj)
        {
            var compareTo = obj as Entity;

            if (ReferenceEquals(this, compareTo))
            {
                return true;
            }

            if (compareTo is null)
            {
                return false;
            }

            return Id.Equals(compareTo.Id);
        }

        public static bool operator !=(Entity? a, Entity? b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return (GetType().GetHashCode() * 907) + Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"{GetType().Name} [Id={Id}]";
        }

        public virtual bool EhValido()
        {
            throw new NotImplementedException();
        }
    }
}

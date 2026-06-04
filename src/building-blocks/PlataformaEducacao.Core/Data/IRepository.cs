using PlataformaEducacao.Core.DomainObjects;

namespace PlataformaEducacao.Core.Data
{

    public interface IRepository<T> : IDisposable where T : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }
    }
}

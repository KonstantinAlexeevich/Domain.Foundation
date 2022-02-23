using System.Threading.Tasks;
using Domain.Foundation.Tactical;

namespace Domain.Foundation.Storage
{
    public interface IAggregateStore<TAggregate, in TIdentity> where TAggregate : IAggregate<TIdentity>
    {
        Task<TAggregate> Create(TIdentity identity);

        Task Store(TAggregate aggregate);
    }
}
using System.Threading.Tasks;
using Domain.Foundation.Tactical;

namespace Domain.Foundation.Core
{
    public interface IAggregateConstructor
    {
        Task<TAggregate> CreateInstanceAsync<TAggregate, TIdentity>(TIdentity id)
            where TAggregate : IAggregate<TIdentity>;
    }
}
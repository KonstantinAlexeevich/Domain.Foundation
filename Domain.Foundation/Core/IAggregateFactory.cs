using System.Threading.Tasks;
using Domain.Foundation.Tactical;

namespace Domain.Foundation.Core
{
    public interface IAggregateFactory
    {
        Task<TAggregate> CreateAggregateInstanceAsync<TAggregate, TIdentity>(TIdentity id)
            where TAggregate : IAggregate<TIdentity>;
    }
}
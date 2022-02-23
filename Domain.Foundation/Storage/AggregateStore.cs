using System.Threading.Tasks;
using Domain.Foundation.Core;
using Domain.Foundation.Tactical;

namespace Domain.Foundation.Storage
{
    public abstract class AggregateStore<TAggregate, TIdentity, TSnapshot> 
        : IAggregateStore<TAggregate, TIdentity> 
        where TAggregate : IAggregate<TIdentity>, IRestoredFrom<TSnapshot>, IStoredTo<TSnapshot>
    {
        private readonly IAggregateConstructor _aggregateConstructor;

        protected AggregateStore(IAggregateConstructor aggregateConstructor)
        {
            _aggregateConstructor = aggregateConstructor;
        }

        public async Task<TAggregate> Create(TIdentity identity)
        {
            var aggregate = await _aggregateConstructor
                .CreateInstanceAsync<TAggregate, TIdentity>(identity)
                .ConfigureAwait(false);

            if (identity != null) 
                await Load(aggregate).ConfigureAwait(false);

            return aggregate;
        }

        public abstract Task Store(TAggregate aggregate);
        protected abstract Task Load(TAggregate aggregate);
    }
}
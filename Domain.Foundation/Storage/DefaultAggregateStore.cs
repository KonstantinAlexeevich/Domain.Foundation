using System.Threading.Tasks;
using Domain.Foundation.Core;
using Domain.Foundation.Tactical;

namespace Domain.Foundation.Storage
{
    public class DefaultAggregateStore<TAggregate, TIdentity>: 
        AggregateStore<TAggregate, TIdentity, Unit> 
        where TAggregate : IAggregate<TIdentity>, IRestoredFrom<Unit>, IStoredTo<Unit>
    {
        public DefaultAggregateStore(IAggregateConstructor aggregateConstructor) : base(aggregateConstructor)
        { }

        public override Task Store(TAggregate aggregate) => Task.CompletedTask;
        protected override Task Load(TAggregate aggregate) => Task.CompletedTask;
    }
}
using System.Threading.Tasks;
using Domain.Foundation.Core;
using Domain.Foundation.CQRS;
using Domain.Foundation.Tactical;

namespace Domain.Foundation.Api
{
    public class ApiAggregateCommandHandler<TAggregate, TIdentity, TCommand, TResult, THandler> 
        : IApiHandler<TCommand, TResult, THandler>
        where THandler : IAggregateCommandHandler<TAggregate, TIdentity, TCommand, TResult>
        where TAggregate : IAggregate<TIdentity>
        where TCommand : ICommand<TIdentity>
    {
        private readonly IAggregateFactory _aggregateFactory;
        private readonly THandler _handler;

        public ApiAggregateCommandHandler(THandler handler, IAggregateFactory aggregateFactory)
        {
            _handler = handler;
            _aggregateFactory = aggregateFactory;
        }

        public async Task<TResult> Handle(TCommand request)
        {
            var aggregate = await _aggregateFactory
                .CreateAggregateInstanceAsync<TAggregate, TIdentity>(request.AggregateId)
                .ConfigureAwait(false);

            await aggregate.LoadAsync().ConfigureAwait(false);

            return await _handler.ExecuteAsync(aggregate, request).ConfigureAwait(false);
        }
    }
}
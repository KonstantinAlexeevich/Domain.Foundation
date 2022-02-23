using System.Threading;
using System.Threading.Tasks;
using Domain.Foundation.CQRS;
using Domain.Foundation.Storage;
using Domain.Foundation.Tactical;

namespace Domain.Foundation.Api
{
    public class ApiAggregateCommandHandler<TAggregate, TIdentity, TCommand, TResult, THandler> 
        : IApiHandler<TCommand, TResult, THandler>
        where THandler : IAggregateCommandHandler<TAggregate, TIdentity, TCommand, TResult>
        where TAggregate : IAggregate<TIdentity>
        where TCommand : ICommand<TIdentity>
    {
        private readonly THandler _handler;
        private readonly IAggregateStore<TAggregate, TIdentity> _aggregateStore;

        public ApiAggregateCommandHandler(THandler handler, IAggregateStore<TAggregate, TIdentity> aggregateStore)
        {
            _handler = handler;
            _aggregateStore = aggregateStore;
        }

        public async Task<ApiResult<TCommand, TResult>> Handle(TCommand request, CancellationToken cancellationToken)
        {
            var aggregate = await _aggregateStore
                .Create(request.AggregateId)
                .ConfigureAwait(false);

            var result = await _handler.ExecuteAsync(aggregate, request, cancellationToken)
                .ConfigureAwait(false);

            await _aggregateStore.Store(aggregate).ConfigureAwait(false);
            
            return new ApiResult<TCommand, TResult>()
            {
                Body = result
            };
        }
    }
}
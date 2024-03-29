﻿using System.Threading;
using System.Threading.Tasks;
using Domain.Foundation.Tactical;

namespace Domain.Foundation.CQRS
{
    public interface IAggregateCommandHandler<in TAggregate, TIdentity, in TCommand> 
        : IAggregateCommandHandler<TAggregate, TIdentity, TCommand, Unit> 
        where TAggregate : IAggregate<TIdentity>
        where TCommand : ICommand<TIdentity>
    {
        new Task ExecuteAsync(TAggregate aggregate, TCommand command, CancellationToken cancellationToken);

        async Task<Unit> IAggregateCommandHandler<TAggregate, TIdentity, TCommand, Unit>.ExecuteAsync(TAggregate aggregate, TCommand command, CancellationToken cancellationToken)
        {
            await ExecuteAsync(aggregate, command, cancellationToken).ConfigureAwait(false);
            return Unit.Value;
        }
    }
    
    public interface IAggregateCommandHandler<in TAggregate, TIdentity, in TCommand, TResult> : IHandler<TCommand, TResult>
        where TAggregate : IAggregate<TIdentity>
        where TCommand : ICommand<TIdentity>
    {
        Task<TResult> ExecuteAsync(TAggregate aggregate, TCommand command, CancellationToken cancellationToken);
    }
}
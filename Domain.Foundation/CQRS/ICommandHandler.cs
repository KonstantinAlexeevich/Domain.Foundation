using System.Threading.Tasks;

namespace Domain.Foundation.CQRS
{
    public interface ICommandHandler<in TCommand> : ICommandHandler<TCommand, Unit> 
        where TCommand : ICommand
    {
        new Task ExecuteAsync(TCommand command);
        async Task<Unit> ICommandHandler<TCommand, Unit>.ExecuteAsync(TCommand command)
        {
            await ExecuteAsync(command).ConfigureAwait(false);
            return Unit.Value;
        }
    }
    
    public interface ICommandHandler<in TCommand, TResult> : IHandler<TCommand, TResult>
        where TCommand : ICommand
    {
        Task<TResult> ExecuteAsync(TCommand command);
    }
}
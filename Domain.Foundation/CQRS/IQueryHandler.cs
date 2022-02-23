using System.Threading;
using System.Threading.Tasks;

namespace Domain.Foundation.CQRS
{
    public interface IQueryHandler<in TRequest, TResponse> : IHandler<TRequest, TResponse>
    {
        Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }

    public interface IQueryHandler<TResponse> : IHandler<Unit, TResponse>
    {
        Task<TResponse> Handle(Unit request, CancellationToken cancellationToken)
        {
            return Handle(cancellationToken);
        }

        Task<TResponse> Handle(CancellationToken cancellationToken);
    }
}
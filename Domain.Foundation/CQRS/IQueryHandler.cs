using System.Threading.Tasks;

namespace Domain.Foundation.CQRS
{
    public interface IQueryHandler<in TRequest, TResponse> : IHandler<TRequest, TResponse>
    {
        Task<TResponse> Handle(TRequest request);
    }

    public interface IQueryHandler<TResponse> : IHandler<Unit, TResponse>
    {
        Task<TResponse> Handle(Unit request)
        {
            return Handle();
        }

        Task<TResponse> Handle();
    }
}
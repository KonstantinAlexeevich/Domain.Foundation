using System.Threading;
using System.Threading.Tasks;
using Domain.Foundation.CQRS;

namespace Domain.Foundation.Api
{
    public interface IApiHandler<TRequest, TResponse, out THandler> : IApiHandler<TRequest, TResponse>
        where THandler : IHandler<TRequest, TResponse>
    {
    }

    public interface IApiHandler<TRequest, TResponse>
    {
        Task<ApiResult<TRequest, TResponse>> Handle(TRequest request, CancellationToken cancellationToken);
    }
}
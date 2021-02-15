using System.Threading.Tasks;
using Domain.Foundation.CQRS;

namespace Domain.Foundation.Api
{
    public interface IApiHandler<in TRequest, TResponse, out THandler> : IApiHandler<TRequest, TResponse>
        where THandler : IHandler<TRequest, TResponse>
    {
    }

    public interface IApiHandler<in TRequest, TResponse>
    {
        Task<TResponse> Handle(TRequest request);
    }
}
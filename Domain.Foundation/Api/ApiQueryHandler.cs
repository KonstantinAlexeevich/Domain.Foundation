using System.Threading.Tasks;
using Domain.Foundation.CQRS;

namespace Domain.Foundation.Api
{
    public class ApiQueryHandler<TRequest, TResponse, THandler> : IApiHandler<TRequest, TResponse, THandler>
        where THandler : IQueryHandler<TRequest, TResponse>
    {
        private readonly THandler _handler;

        public ApiQueryHandler(THandler handler)
        {
            _handler = handler;
        }

        public Task<TResponse> Handle(TRequest request)
        {
            return _handler.Handle(request);
        }
    }
}
using System.Threading;
using System.Threading.Tasks;
using Domain.Foundation.Api;
using Domain.Foundation.CQRS;

namespace Domain.Foundation.Tests
{
    public class TestApiHandlerDecorator<TRequest, TResponse, THandler>: IApiHandler<TRequest, TResponse, THandler>
        where THandler : IHandler<TRequest, TResponse>
    {
        private readonly IApiHandler<TRequest, TResponse, THandler> _decorated;
        private readonly THandler _handler;

        public TestApiHandlerDecorator(IApiHandler<TRequest, TResponse, THandler> decorated, THandler handler)
        {
            _decorated = decorated;
            _handler = handler;
        }
        
        public Task<ApiResult<TRequest, TResponse>> Handle(TRequest request, CancellationToken cancellationToken)
        { 
            return _decorated.Handle(request, cancellationToken);
        }
    }
}
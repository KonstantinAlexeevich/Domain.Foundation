using System.Threading;
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

        public async Task<ApiResult<TRequest, TResponse>> Handle(TRequest request, CancellationToken cancellationToken)
        {
            var result =  await _handler.Handle(request, cancellationToken);
            
            return new ApiResult<TRequest, TResponse>()
            {
                Body = result
            };
        }
    }
}
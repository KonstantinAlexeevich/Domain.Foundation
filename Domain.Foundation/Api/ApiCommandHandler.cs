using System.Threading;
using System.Threading.Tasks;
using Domain.Foundation.CQRS;

namespace Domain.Foundation.Api
{
    public class ApiCommandHandler<TRequest, TResponse, THandler> : IApiHandler<TRequest, TResponse, THandler>
        where THandler : ICommandHandler<TRequest, TResponse>
        where TRequest : ICommand
    {
        private readonly THandler _handler;

        public ApiCommandHandler(THandler handler)
        {
            _handler = handler;
        }

        public async Task<ApiResult<TRequest, TResponse>> Handle(TRequest request, CancellationToken cancellationToken)
        {
            var result = await _handler.ExecuteAsync(request, cancellationToken);
            
            return new ApiResult<TRequest, TResponse>()
            {
                Body = result
            };
        }
    }
}
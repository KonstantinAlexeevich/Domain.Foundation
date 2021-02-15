using Domain.Foundation.Api;
using Domain.Foundation.CQRS;

namespace Domain.Foundation.Tests
{
    public class ApiHandlersResult<TRequest, TResponse, TMarkerInterface>
        where TMarkerInterface : IHandler<TRequest, TResponse>
    {
        public IApiHandler<TRequest, TResponse, IHandler<TRequest, TResponse>> ApiHandlerByIHandlerMarker { get; set; }
        public IApiHandler<TRequest, TResponse, TMarkerInterface> ApiHandlerByMarker { get; set; }
        public IApiHandler<TRequest, TResponse> ApiHandler { get; set; }
    }
}
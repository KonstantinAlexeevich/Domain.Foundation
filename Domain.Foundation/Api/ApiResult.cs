using System.Collections;
using System.Collections.Generic;

namespace Domain.Foundation.Api
{
    public record ApiResult<TRequest, TResponse>
    {
        public TResponse Body { get; init; }
        public ICollection<Error> Errors { get; set; }
    }

    public record Error
    {
        public string Code { get; init; }
        public string Message { get; init; }
        public string Additional { get; init; }
    }
}
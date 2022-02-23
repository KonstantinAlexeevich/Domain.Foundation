using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Foundation.Api;

namespace Domain.Foundation
{
    public static class TaskResponseExtensions
    {
        public static TaskResponseHelper<TRequest, TResponse> OnErrors<TRequest, TResponse>(this Task<ApiResult<TRequest, TResponse>> task,
            Action<ICollection<Error>> errorsHandle) =>
            TaskResponseHelper<TRequest, TResponse>.FromErrorsHandle(task, errorsHandle);

        public static TaskResponseHelper<TRequest, TResponse> OnError<TRequest, TResponse>(this Task<ApiResult<TRequest, TResponse>> task,
            Action<string> errorHandle) =>
            TaskResponseHelper<TRequest, TResponse>.FromErrorHandle(task, errorHandle);

        public static TaskResponseHelper<TRequest, TResponse> OnSuccess<TRequest, TResponse>(this Task<ApiResult<TRequest, TResponse>> task,
            Action<TResponse> responseHandle) =>
            TaskResponseHelper<TRequest, TResponse>.FromSuccessHandle(task, responseHandle);

        public static TaskResponseHelper<TRequest, TResponse> OnSuccess<TRequest, TResponse>(this Task<ApiResult<TRequest, TResponse>> task,
            Func<TResponse, Task> responseHandle) =>
            TaskResponseHelper<TRequest, TResponse>.FromSuccessHandle(task, responseHandle);

        public static TaskResponseHelper<TRequest, TResponse> OnSuccess<TRequest, TResponse>(this Task<ApiResult<TRequest, TResponse>> task,
            Action responseHandle) =>
            TaskResponseHelper<TRequest, TResponse>.FromSuccessHandle(task, responseHandle);

        public static TaskResponseHelper<TRequest, TResponse> OnComplete<TRequest, TResponse>(this Task<ApiResult<TRequest, TResponse>> task,
            Action<ApiResult<TRequest, TResponse>> completeHandle) =>
            TaskResponseHelper<TRequest, TResponse>.FromComplete(task, completeHandle);
    }
}
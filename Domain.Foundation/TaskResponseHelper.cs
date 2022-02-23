using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Domain.Foundation.Api;

namespace Domain.Foundation
{
    public class TaskResponseHelper<TRequest, TResponse>
    {
        public TaskResponseHelper(Task<ApiResult<TRequest, TResponse>> task) => _task = task;

        readonly List<Action<ApiResult<TRequest, TResponse>>> _completeHandle = new List<Action<ApiResult<TRequest, TResponse>>>();
        readonly List<Action<ICollection<Error>>> _errorsHandle = new List<Action<ICollection<Error>>>();
        readonly List<Func<TResponse, Task>> _responseAsyncHandle = new List<Func<TResponse, Task>>();
        readonly List<Action<TResponse>> _responseHandle = new List<Action<TResponse>>();
        readonly Task<ApiResult<TRequest, TResponse>> _task;

        public static TaskResponseHelper<TRequest, TResponse> FromErrorsHandle(Task<ApiResult<TRequest, TResponse>> task,
            Action<ICollection<Error>> errorsHandle) =>
            new TaskResponseHelper<TRequest, TResponse>(task)
                .OnErrors(errorsHandle);

        public static TaskResponseHelper<TRequest, TResponse> FromErrorHandle(Task<ApiResult<TRequest, TResponse>> task,
            Action<string> errorHandle) =>
            new TaskResponseHelper<TRequest, TResponse>(task)
                .OnError(errorHandle);

        public static TaskResponseHelper<TRequest, TResponse> FromSuccessHandle(Task<ApiResult<TRequest, TResponse>> task,
            Action<TResponse> responseHandle) =>
            new TaskResponseHelper<TRequest, TResponse>(task)
                .OnSuccess(responseHandle);

        public static TaskResponseHelper<TRequest, TResponse> FromSuccessHandle(Task<ApiResult<TRequest, TResponse>> task,
            Func<TResponse, Task> responseHandle) =>
            new TaskResponseHelper<TRequest, TResponse>(task)
                .OnSuccess(responseHandle);

        public static TaskResponseHelper<TRequest, TResponse> FromSuccessHandle(Task<ApiResult<TRequest, TResponse>> task, Action responseHandle) =>
            new TaskResponseHelper<TRequest, TResponse>(task)
                .OnSuccess(responseHandle);

        public static TaskResponseHelper<TRequest, TResponse> FromComplete(Task<ApiResult<TRequest, TResponse>> task, Action<ApiResult<TRequest, TResponse>> responseHandle) =>
            new TaskResponseHelper<TRequest, TResponse>(task)
                .OnComplete(responseHandle);


        public TaskResponseHelper<TRequest, TResponse> OnErrors(Action<ICollection<Error>> errorsHandle)
        {
            _errorsHandle.Add(errorsHandle);
            return this;
        }

        public TaskResponseHelper<TRequest, TResponse> OnError(Action<string> errorHandle)
        {
            _errorsHandle.Add(x => errorHandle(string.Join(' ', x.Select(y => y.Message))));
            return this;
        }

        public TaskResponseHelper<TRequest, TResponse> OnSuccess(Action<TResponse> responseHandle)
        {
            _responseHandle.Add(responseHandle);
            return this;
        }

        public TaskResponseHelper<TRequest, TResponse> OnSuccess(Func<TResponse, Task> responseHandle)
        {
            _responseAsyncHandle.Add(responseHandle);
            return this;
        }

        public TaskResponseHelper<TRequest, TResponse> OnSuccess(Action responseHandle)
        {
            _responseHandle.Add(x => responseHandle());
            return this;
        }

        public TaskResponseHelper<TRequest, TResponse> OnComplete(Action<ApiResult<TRequest, TResponse>> completeHandle)
        {
            _completeHandle.Add(completeHandle);
            return this;
        }

        public static implicit operator Task<ApiResult<TRequest, TResponse>>(TaskResponseHelper<TRequest, TResponse> response) => response.Execute();

        public TaskAwaiter<ApiResult<TRequest, TResponse>> GetAwaiter() => Execute().GetAwaiter();

        async Task<ApiResult<TRequest, TResponse>> Execute()
        {
            var response = await _task;
            if (response.Errors != null && response.Errors.Any())
                _errorsHandle.ForEach(x => x.Invoke(response.Errors.ToList()));
            else
            {
                _responseHandle.ForEach(x => x.Invoke(response.Body));

                foreach (var asyncHandlers in _responseAsyncHandle)
                    await asyncHandlers.Invoke(response.Body);
            }

            _completeHandle.ForEach(x => x.Invoke(response));
            return response;
        }
    }
}
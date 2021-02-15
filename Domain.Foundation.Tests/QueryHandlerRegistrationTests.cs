using System.Threading.Tasks;
using Domain.Foundation.Api;
using Domain.Foundation.CQRS;
using Domain.Foundation.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Domain.Foundation.Tests
{
    public class QueryHandlerRegistrationTests
    {
        readonly RegistrationTests _tests = new RegistrationTests();
        
        [Fact]
        public void Registration_WithQueryHandlerMarkerInterface_ShouldBeSuccess()
        {
            _tests.HandlersAndApiHandlers_ShouldBeRegistered<
                TestA.Request,
                TestA.Response,
                TestA.ITestHandler,
                IQueryHandler<TestA.Request, TestA.Response>
            >();
        }
        
        [Fact]
        public void WithQueryHandlerMarkerInterface_IsDecorated()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDomainFoundation(x => 
                x.AddAssemblies(GetType().Assembly)
                    .DecorateApiHandler(typeof(TestApiHandlerDecorator<,,>)));
            
            var serviceProvider = serviceCollection.BuildServiceProvider();
            
            var apiHandlers = GetApiHandlers<
                TestA.Request,
                TestA.Response,
                TestA.ITestHandler
            >(serviceProvider);

            var decorated = typeof(TestApiHandlerDecorator<TestA.Request, TestA.Response, TestA.ITestHandler>);
            Assert.Equal(decorated, apiHandlers.ApiHandler.GetType());
            Assert.Equal(decorated, apiHandlers.ApiHandlerByMarker.GetType());
            Assert.Equal(decorated, apiHandlers.ApiHandlerByIHandlerMarker.GetType());
        }
        
        [Fact]
        public void Registration_WithHandlerMarkerInterface_ShouldBeSuccess()
        {
            _tests.HandlersAndApiHandlers_ShouldBeRegistered<
                TestB.Request,
                TestB.Response,
                TestB.ITestHandler,
                IQueryHandler<TestB.Request, TestB.Response>
            >();
        }
        
        [Fact]
        public void WithHandlerMarkerInterface_IsDecorated()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDomainFoundation(x => 
                x.AddAssemblies(GetType().Assembly)
                    .DecorateApiHandler(typeof(TestApiHandlerDecorator<,,>)));
            
            var serviceProvider = serviceCollection.BuildServiceProvider();
            
            var apiHandlers = GetApiHandlers<
                TestB.Request,
                TestB.Response,
                TestB.ITestHandler
            >(serviceProvider);

            var decorated = typeof(TestApiHandlerDecorator<TestB.Request, TestB.Response, TestB.ITestHandler>);
            Assert.Equal(decorated, apiHandlers.ApiHandler.GetType());
            Assert.Equal(decorated, apiHandlers.ApiHandlerByMarker.GetType());
            Assert.Equal(decorated, apiHandlers.ApiHandlerByIHandlerMarker.GetType());
        }
        
        [Fact]
        public void Registration_WithoutMarkerInterface_ShouldBeSuccess()
        {
            _tests.HandlersAndApiHandlers_ShouldBeRegistered<
                TestC.Request,
                TestC.Response,
                IHandler<TestC.Request, TestC.Response>,
                IQueryHandler<TestC.Request, TestC.Response>
            >();
        }
        
        [Fact]
        public void WithoutMarkerInterface_IsDecorated()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDomainFoundation(x => 
                x.AddAssemblies(GetType().Assembly)
                    .DecorateApiHandler(typeof(TestApiHandlerDecorator<,,>)));
            
            var serviceProvider = serviceCollection.BuildServiceProvider();
            
            var apiHandlers = GetApiHandlers<
                TestC.Request,
                TestC.Response,
                IHandler<TestC.Request, TestC.Response>
            >(serviceProvider);

            var decorated = typeof(TestApiHandlerDecorator<TestC.Request, TestC.Response, IHandler<TestC.Request, TestC.Response>>);
            Assert.Equal(decorated, apiHandlers.ApiHandler.GetType());
            Assert.Equal(decorated, apiHandlers.ApiHandlerByMarker.GetType());
            Assert.Equal(decorated, apiHandlers.ApiHandlerByIHandlerMarker.GetType());
        }
        
        ApiHandlersResult<TRequest, TResponse, TMarkerInterface> GetApiHandlers<TRequest, TResponse, TMarkerInterface>(ServiceProvider serviceProvider)
            where TMarkerInterface : IHandler<TRequest, TResponse>
        {
            var apiHandlerByIHandlerMarker = serviceProvider.GetRequiredService<IApiHandler<TRequest, TResponse, IHandler<TRequest, TResponse>>>();
            var apiHandlerByMarker = serviceProvider.GetRequiredService<IApiHandler<TRequest, TResponse, TMarkerInterface>>();
            var apiHandler = serviceProvider.GetRequiredService<IApiHandler<TRequest, TResponse>>();

            return new ApiHandlersResult<TRequest, TResponse, TMarkerInterface>
            {
                ApiHandlerByIHandlerMarker = apiHandlerByIHandlerMarker,
                ApiHandlerByMarker = apiHandlerByMarker,
                ApiHandler = apiHandler
            };
        }

        public static class TestA
        {
    
            public interface ITestHandler: IQueryHandler<Request, Response>
            {
            }

            public class Request
            {
            }
        
            public class Response
            {
            }
        
            public class TestHandler : ITestHandler
            {
                public Task<Response> Handle(Request request)
                {
                    return Task.FromResult(new Response());
                }
            }
        }
        public static class TestB
        {
    
            public interface ITestHandler: IHandler<Request, Response>
            {
            }

            public class Request
            {
            }
        
            public class Response
            {
            }
        
            public class TestHandler : ITestHandler, IQueryHandler<Request, Response>
            {
                public Task<Response> Handle(Request request)
                {
                    return Task.FromResult(new Response());
                }
            }
        }
        public static class TestC
        {
            public class Request
            {
            }
        
            public class Response
            {
            }
        
            public class TestHandler : IQueryHandler<Request, Response>
            {
                public Task<Response> Handle(Request request)
                {
                    return Task.FromResult(new Response());
                }
            }
        }
    }
}
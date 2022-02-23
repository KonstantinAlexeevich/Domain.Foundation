using System.Threading;
using System.Threading.Tasks;
using Domain.Foundation.CQRS;
using Domain.Foundation.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Domain.Foundation.Tests
{
    public class CommandHandlerRegistrationTests
    {
        readonly RegistrationTestsHelper _testsHelper = new RegistrationTestsHelper();
        
        [Fact]
        public void Registration_WithQueryHandlerMarkerInterface_ShouldBeSuccess()
        {
            _testsHelper.HandlersAndApiHandlers_ShouldBeRegistered<
                TestA.Request,
                TestA.Response,
                TestA.ITestHandler,
                ICommandHandler<TestA.Request, TestA.Response>
            >();
        }
        
        [Fact]
        public void Registration_WithHandlerMarkerInterface_ShouldBeSuccess()
        {
            _testsHelper.HandlersAndApiHandlers_ShouldBeRegistered<
                TestB.Request,
                TestB.Response,
                TestB.ITestHandler,
                ICommandHandler<TestB.Request, TestB.Response>
            >();
        }
        
        [Fact]
        public void Registration_WithoutMarkerInterface_ShouldBeSuccess()
        {
            _testsHelper.HandlersAndApiHandlers_ShouldBeRegistered<
                TestC.Request,
                TestC.Response,
                IHandler<TestC.Request, TestC.Response>,
                ICommandHandler<TestC.Request, TestC.Response>
            >();
        }
        
        [Fact]
        public void WithQueryHandlerMarkerInterface_IsDecorated()
        {
            var serviceProvider = _testsHelper.GetServiceProviderWithApiHandlerDecorator();
            
            var apiHandlers = _testsHelper.GetApiHandlers<
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
        public void WithHandlerMarkerInterface_IsDecorated()
        {
            var serviceProvider = _testsHelper.GetServiceProviderWithApiHandlerDecorator();
            
            var apiHandlers = _testsHelper.GetApiHandlers<
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
        public void WithoutMarkerInterface_IsDecorated()
        {
            var serviceProvider = _testsHelper.GetServiceProviderWithApiHandlerDecorator();

            var apiHandlers = _testsHelper.GetApiHandlers<
                TestC.Request,
                TestC.Response,
                IHandler<TestC.Request, TestC.Response>
            >(serviceProvider);

            var decorated = typeof(TestApiHandlerDecorator<TestC.Request, TestC.Response, IHandler<TestC.Request, TestC.Response>>);
            Assert.Equal(decorated, apiHandlers.ApiHandler.GetType());
            Assert.Equal(decorated, apiHandlers.ApiHandlerByMarker.GetType());
            Assert.Equal(decorated, apiHandlers.ApiHandlerByIHandlerMarker.GetType());
        }

        public static class TestA
        {
    
            public interface ITestHandler: ICommandHandler<Request, Response>
            {
            }

            public class Request : ICommand
            {
            }
        
            public class Response
            {
            }
        
            public class TestHandler : ITestHandler
            {
                public Task<Response> ExecuteAsync(Request command, CancellationToken cancellationToken)
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

            public class Request: ICommand
            {
            }
        
            public class Response
            {
            }
        
            public class TestHandler : ITestHandler, ICommandHandler<Request, Response>
            {
                public Task<Response> ExecuteAsync(Request command, CancellationToken cancellationToken)
                {
                    return Task.FromResult(new Response());
                }
            }
        }
        public static class TestC
        {
            public class Request: ICommand
            {
            }
        
            public class Response
            {
            }
        
            public class TestHandler : ICommandHandler<Request, Response>
            {
                public Task<Response> ExecuteAsync(Request command, CancellationToken cancellationToken)
                {
                    return Task.FromResult(new Response());
                }
            }
        }
    }
}
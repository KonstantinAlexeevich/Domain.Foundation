using System.Threading.Tasks;
using Domain.Foundation.CQRS;
using Xunit;

namespace Domain.Foundation.Tests
{
    public class CommandHandlerRegistrationTests
    {
        readonly RegistrationTests _tests = new RegistrationTests();
        
        [Fact]
        public void Registration_WithQueryHandlerMarkerInterface_ShouldBeSuccess()
        {
            _tests.HandlersAndApiHandlers_ShouldBeRegistered<
                TestA.Request,
                TestA.Response,
                TestA.ITestHandler,
                ICommandHandler<TestA.Request, TestA.Response>
            >();
        }
        
        [Fact]
        public void Registration_WithHandlerMarkerInterface_ShouldBeSuccess()
        {
            _tests.HandlersAndApiHandlers_ShouldBeRegistered<
                TestB.Request,
                TestB.Response,
                TestB.ITestHandler,
                ICommandHandler<TestB.Request, TestB.Response>
            >();
        }
        
        [Fact]
        public void Registration_WithoutMarkerInterface_ShouldBeSuccess()
        {
            _tests.HandlersAndApiHandlers_ShouldBeRegistered<
                TestC.Request,
                TestC.Response,
                IHandler<TestC.Request, TestC.Response>,
                ICommandHandler<TestC.Request, TestC.Response>
            >();
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
                public Task<Response> ExecuteAsync(Request command)
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
                public Task<Response> ExecuteAsync(Request command)
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
                public Task<Response> ExecuteAsync(Request command)
                {
                    return Task.FromResult(new Response());
                }
            }
        }
    }
}
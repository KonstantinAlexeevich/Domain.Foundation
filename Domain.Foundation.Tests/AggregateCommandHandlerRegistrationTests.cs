using System.Threading.Tasks;
using Domain.Foundation.CQRS;
using Domain.Foundation.Tactical;
using Xunit;

namespace Domain.Foundation.Tests
{
    public class AggregateCommandHandlerRegistrationTests
    {
        readonly RegistrationTests _tests = new RegistrationTests();
        
        [Fact]
        public void Registration_WithQueryHandlerMarkerInterface_ShouldBeSuccess()
        {
            _tests.HandlersAndApiHandlers_ShouldBeRegistered<
                TestA.Request,
                TestA.Response,
                TestA.ITestHandler,
                IAggregateCommandHandler<TestAggregate, long, TestA.Request, TestA.Response>
            >();
        }
        
        
        [Fact]
        public void Registration_WithHandlerMarkerInterface_ShouldBeSuccess()
        {
            _tests.HandlersAndApiHandlers_ShouldBeRegistered<
                TestB.Request,
                TestB.Response,
                TestB.ITestHandler,
                IAggregateCommandHandler<TestAggregate, long, TestB.Request, TestB.Response>
            >();
        }
        
        [Fact]
        public void Registration_WithoutMarkerInterface_ShouldBeSuccess()
        {
            _tests.HandlersAndApiHandlers_ShouldBeRegistered<
                TestC.Request,
                TestC.Response,
                IHandler<TestC.Request, TestC.Response>,
                IAggregateCommandHandler<TestAggregate, long, TestC.Request, TestC.Response>
            >();
        }

        public static class TestA
        {
    
            public interface ITestHandler: IAggregateCommandHandler<TestAggregate, long, Request, Response>
            {
            }

            public class Request : ICommand<long>
            {
                public long AggregateId { get; set; }
            }
        
            public class Response
            {
            }
        
            public class TestHandler : ITestHandler
            {
                public Task<Response> ExecuteAsync(TestAggregate aggregate, Request command)
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

            public class Request: ICommand<long>
            {
                public long AggregateId { get; set; }
            }
        
            public class Response
            {
            }
        
            public class TestHandler : ITestHandler, IAggregateCommandHandler<TestAggregate, long, Request, Response>
            {
                public Task<Response> ExecuteAsync(TestAggregate aggregate, Request command)
                {
                    return Task.FromResult(new Response());
                }
            }
        }
        public static class TestC
        {
            public class Request: ICommand<long>
            {
                public long AggregateId { get; set; }
            }
        
            public class Response
            {
            }
        
            public class TestHandler : IAggregateCommandHandler<TestAggregate, long, Request, Response>
            {
                public Task<Response> ExecuteAsync(TestAggregate aggregate, Request command)
                {
                    return Task.FromResult(new Response());
                }
            }
        }

        public class TestAggregate : IAggregate<long>
        {
        }
    }
}
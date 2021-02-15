using Domain.Foundation.Api;
using Domain.Foundation.CQRS;
using Domain.Foundation.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Domain.Foundation.Tests
{
    public class RegistrationTests
    {
        /// <summary>
        /// Test that register
        /// * IHandler<![CDATA[<TRequest, TResponse>]]>
        /// * THandler
        /// * TMarkerInterface
        /// * IApiHandler<![CDATA[<TRequest, TResponse, THandler>]]>
        /// * IApiHandler<![CDATA[<TRequest, TResponse, IHandler<TRequest, TResponse>>]]>
        /// * IApiHandler<![CDATA[<TRequest, TResponse, TMarkerInterface>]]>
        /// * IApiHandler<![CDATA[<TRequest, TResponse>]]>
        /// </summary>
        /// <typeparam name="TRequest">Request</typeparam>
        /// <typeparam name="TResponse">Response</typeparam>
        /// <typeparam name="TMarkerInterface">Marker Interface of concrete THandler or IHandler</typeparam>
        /// <typeparam name="THandler">Generic Interface of concrete IHandler</typeparam>
        public void HandlersAndApiHandlers_ShouldBeRegistered<TRequest, TResponse, TMarkerInterface, THandler>()
            where THandler : IHandler<TRequest, TResponse>
            where TMarkerInterface : IHandler<TRequest, TResponse>
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDomainFoundation(x => x.AddAssemblies(GetType().Assembly));
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var handler = serviceProvider.GetRequiredService<IHandler<TRequest, TResponse>>();
            var queryHandler = serviceProvider.GetRequiredService<THandler>();
            var handlerByMarker = serviceProvider.GetRequiredService<TMarkerInterface>();
            
            Assert.Equal(handler, queryHandler);
            Assert.Equal(handler, handlerByMarker);
            
            var apiHandlerByIHandlerMarker = serviceProvider.GetRequiredService<IApiHandler<TRequest, TResponse, IHandler<TRequest, TResponse>>>();
            var apiHandlerByMarker = serviceProvider.GetRequiredService<IApiHandler<TRequest, TResponse, TMarkerInterface>>();
            var apiHandler = serviceProvider.GetRequiredService<IApiHandler<TRequest, TResponse>>();
            
            Assert.Equal(apiHandler, apiHandlerByIHandlerMarker);
            Assert.Equal(apiHandler, apiHandlerByMarker);
        }
    }
}
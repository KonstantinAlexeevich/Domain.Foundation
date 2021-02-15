using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Domain.Foundation.Api;
using Domain.Foundation.CQRS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Domain.Foundation.DependencyInjection
{
    public class RegistrationHelper
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Type IHandlerInterface = typeof(IHandler<,>);
        private static readonly Type ApiMarkerHandlerInterface = typeof(IApiHandler<,,>);
        private static readonly Type ApiHandlerInterface = typeof(IApiHandler<,>);
        private readonly Func<Type, Type> _generateApiHandlerCallback;
        private readonly List<Type> _genericDecorators;

        private readonly Type _handlerGenericInterface;
        private readonly Type _tRequest;
        private readonly Type _tResponse;

        public RegistrationHelper(Type handlerGenericInterface, Type tRequest, Type tResponse, Func<Type, Type> generateApiHandlerCallback, List<Type> genericDecorators)
        {
            _handlerGenericInterface = handlerGenericInterface;
            _tRequest = tRequest;
            _tResponse = tResponse;
            _generateApiHandlerCallback = generateApiHandlerCallback;
            _genericDecorators = genericDecorators;
        }

        private Type GetMarkerApiHandlerType(Type markerInterface)
        {
            return _generateApiHandlerCallback.Invoke(markerInterface);
        }

        public void Register(IServiceCollection serviceCollection, TypeInfo handlerType)
        {
            var handlerGenericInterface = handlerType.GetGenericType(_handlerGenericInterface);
            
            var handlerMarkerInterface = handlerType
                .GetMarkerInterfaces(IHandlerInterface)
                .FirstOrDefault(x => x != handlerGenericInterface);
            
            Type concreteApiHandler;

            var iHandlerInterface = IHandlerInterface.MakeGenericType(_tRequest, _tResponse);

            if (handlerMarkerInterface != null)
            {
                serviceCollection.AddScoped(handlerType);
                serviceCollection.AddScoped(handlerMarkerInterface, y => y.GetRequiredService(handlerType));
                serviceCollection.AddScoped(handlerGenericInterface, y => y.GetRequiredService(handlerType));
                serviceCollection.AddScoped(iHandlerInterface, y => y.GetRequiredService(handlerType));
                
                var isMarkerInterfaceImplementsGenericHandler = handlerMarkerInterface.ImplementsGenericInterface(_handlerGenericInterface);

                if (isMarkerInterfaceImplementsGenericHandler)
                {
                    var apiHandlerTypeWithMarker = GetMarkerApiHandlerType(handlerMarkerInterface);
                    var apiHandlerInterfaceWithMarker = ApiMarkerHandlerInterface.MakeGenericType(_tRequest, _tResponse, handlerMarkerInterface);

                    serviceCollection.AddScoped(apiHandlerInterfaceWithMarker, apiHandlerTypeWithMarker);
                    DecorateApiHandler(serviceCollection, apiHandlerInterfaceWithMarker, handlerMarkerInterface);

                    var apiHandlerWithIHandlerMarker = ApiMarkerHandlerInterface.MakeGenericType(_tRequest, _tResponse, iHandlerInterface);
                    serviceCollection.AddScoped(apiHandlerWithIHandlerMarker, y => y.GetRequiredService(apiHandlerInterfaceWithMarker));
                    
                    concreteApiHandler = apiHandlerInterfaceWithMarker;
                }
                else
                {
                    var apiHandlerTypeWithHandlerMarker = GetMarkerApiHandlerType(handlerType);
                    var apiHandlerInterfaceWithHandlerMarker = ApiMarkerHandlerInterface.MakeGenericType(_tRequest, _tResponse, handlerType);
                    
                    serviceCollection.AddScoped(apiHandlerInterfaceWithHandlerMarker, apiHandlerTypeWithHandlerMarker);
                    DecorateApiHandler(serviceCollection, apiHandlerInterfaceWithHandlerMarker, handlerMarkerInterface);
                    
                    var apiHandlerWithIHandlerMarker = ApiMarkerHandlerInterface.MakeGenericType(_tRequest, _tResponse, iHandlerInterface);
                    serviceCollection.AddScoped(apiHandlerWithIHandlerMarker, y=> y.GetRequiredService(apiHandlerInterfaceWithHandlerMarker));

                    var apiHandlerInterfaceWithMarker = ApiMarkerHandlerInterface.MakeGenericType(_tRequest, _tResponse, handlerMarkerInterface);
                    serviceCollection.AddScoped(apiHandlerInterfaceWithMarker, y=> y.GetRequiredService(apiHandlerInterfaceWithHandlerMarker));
                    
                    concreteApiHandler = apiHandlerInterfaceWithMarker;
                }
            }
            else
            {
                serviceCollection.AddScoped(handlerGenericInterface, handlerType);
                serviceCollection.AddScoped(iHandlerInterface, y => y.GetRequiredService(handlerGenericInterface));

                var apiHandlerTypeWithIHandlerMarker = GetMarkerApiHandlerType(handlerGenericInterface);
                var apiHandlerInterfaceWithIHandlerMarker = ApiMarkerHandlerInterface.MakeGenericType(_tRequest, _tResponse, iHandlerInterface);

                serviceCollection.AddScoped(apiHandlerInterfaceWithIHandlerMarker, apiHandlerTypeWithIHandlerMarker);
                DecorateApiHandler(serviceCollection, apiHandlerInterfaceWithIHandlerMarker, iHandlerInterface);
                
                concreteApiHandler = apiHandlerInterfaceWithIHandlerMarker;
            }

            var apiHandlerInterface = ApiHandlerInterface.MakeGenericType(_tRequest, _tResponse);
            serviceCollection.AddScoped(apiHandlerInterface, y => y.GetRequiredService(concreteApiHandler));
        }

        void DecorateApiHandler(IServiceCollection serviceCollection, Type markedApiHandlerInterface, Type markerInterface)
        {
            foreach (var genericDecorator in _genericDecorators)
            {
                var descriptor = serviceCollection.Single(x => x.ServiceType == markedApiHandlerInterface);
                
                var arguments = genericDecorator.GetGenericArguments();
                Type decorator = null;
                
                if (arguments.Length == 3)
                    decorator = genericDecorator.MakeGenericType(_tRequest, _tResponse, markerInterface);
                else
                    decorator = genericDecorator.MakeGenericType(_tRequest, _tResponse);
                
                var objectFactory = ActivatorUtilities.CreateFactory(decorator, new[] { markedApiHandlerInterface });
                
                serviceCollection.Replace(ServiceDescriptor.Describe(
                    markedApiHandlerInterface, 
                    x => objectFactory.Invoke(x, new [] { CreateInstance(x, descriptor) }), 
                    descriptor.Lifetime)
                );
            }
        }
        
        private static object CreateInstance(IServiceProvider services, ServiceDescriptor descriptor)
        {
            if (descriptor.ImplementationInstance != null)
                return descriptor.ImplementationInstance;

            if (descriptor.ImplementationFactory != null)
                return descriptor.ImplementationFactory(services);

            return ActivatorUtilities.GetServiceOrCreateInstance(services, descriptor.ImplementationType);
        }
    }
}
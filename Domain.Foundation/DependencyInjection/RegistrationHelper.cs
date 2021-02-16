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
    internal class RegistrationHelper
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Type IHandlerInterface = typeof(IHandler<,>);
        private static readonly Type ApiMarkerHandlerInterface = typeof(IApiHandler<,,>);
        private static readonly Type ApiHandlerInterface = typeof(IApiHandler<,>);
        
        private readonly List<Type> _genericDecorators;
        private readonly Type _handlerGenericInterface;
        private readonly IServiceCollection _serviceCollection;
        
        Func<Type, Type> _generateApiHandlerCallback;

        public RegistrationHelper(
            IServiceCollection serviceCollection,
            Type handlerGenericInterface,
            List<Type> genericDecorators)
        {
            _serviceCollection = serviceCollection;
            _handlerGenericInterface = handlerGenericInterface;
            _genericDecorators = genericDecorators;
        }

        public RegistrationHelper WithGetApiHandler(Func<Type, Type> generateApiHandlerCallback)
        {
            _generateApiHandlerCallback = generateApiHandlerCallback;
            return this;
        }
        
        private Type GetMarkerApiHandlerType(Type markerInterface)
        {
            return _generateApiHandlerCallback.Invoke(markerInterface);
        }

        public void Register(TypeInfo handlerType, Type tRequest, Type tResponse)
        {
            var handlerGenericInterface = handlerType.GetGenericType(_handlerGenericInterface);

            var handlerMarkerInterface = handlerType
                .GetMarkerInterfaces(IHandlerInterface)
                .FirstOrDefault(x => x != handlerGenericInterface);

            Type concreteApiHandler;

            var iHandlerInterface = IHandlerInterface.MakeGenericType(tRequest, tResponse);

            if (handlerMarkerInterface != null)
            {
                _serviceCollection.AddScoped(handlerType);
                _serviceCollection.AddScoped(handlerMarkerInterface, y => y.GetRequiredService(handlerType));
                _serviceCollection.AddScoped(handlerGenericInterface, y => y.GetRequiredService(handlerType));
                _serviceCollection.AddScoped(iHandlerInterface, y => y.GetRequiredService(handlerType));

                var isMarkerInterfaceImplementsGenericHandler =
                    handlerMarkerInterface.ImplementsGenericInterface(_handlerGenericInterface);

                if (isMarkerInterfaceImplementsGenericHandler)
                {
                    var apiHandlerTypeWithMarker = GetMarkerApiHandlerType(handlerMarkerInterface);
                    var apiHandlerInterfaceWithMarker =
                        ApiMarkerHandlerInterface.MakeGenericType(tRequest, tResponse, handlerMarkerInterface);

                    _serviceCollection.AddScoped(apiHandlerInterfaceWithMarker, apiHandlerTypeWithMarker);
                    DecorateApiHandler(apiHandlerInterfaceWithMarker, tRequest, tResponse, handlerMarkerInterface);

                    var apiHandlerWithIHandlerMarker =
                        ApiMarkerHandlerInterface.MakeGenericType(tRequest, tResponse, iHandlerInterface);
                    _serviceCollection.AddScoped(apiHandlerWithIHandlerMarker,
                        y => y.GetRequiredService(apiHandlerInterfaceWithMarker));

                    concreteApiHandler = apiHandlerInterfaceWithMarker;
                }
                else
                {
                    var apiHandlerTypeWithHandlerMarker = GetMarkerApiHandlerType(handlerType);
                    var apiHandlerInterfaceWithHandlerMarker =
                        ApiMarkerHandlerInterface.MakeGenericType(tRequest, tResponse, handlerType);

                    _serviceCollection.AddScoped(apiHandlerInterfaceWithHandlerMarker, apiHandlerTypeWithHandlerMarker);
                    DecorateApiHandler(apiHandlerInterfaceWithHandlerMarker, tRequest, tResponse, handlerMarkerInterface);

                    var apiHandlerWithIHandlerMarker =
                        ApiMarkerHandlerInterface.MakeGenericType(tRequest, tResponse, iHandlerInterface);
                    _serviceCollection.AddScoped(apiHandlerWithIHandlerMarker,
                        y => y.GetRequiredService(apiHandlerInterfaceWithHandlerMarker));

                    var apiHandlerInterfaceWithMarker =
                        ApiMarkerHandlerInterface.MakeGenericType(tRequest, tResponse, handlerMarkerInterface);
                    _serviceCollection.AddScoped(apiHandlerInterfaceWithMarker,
                        y => y.GetRequiredService(apiHandlerInterfaceWithHandlerMarker));

                    concreteApiHandler = apiHandlerInterfaceWithMarker;
                }
            }
            else
            {
                _serviceCollection.AddScoped(handlerGenericInterface, handlerType);
                _serviceCollection.AddScoped(iHandlerInterface, y => y.GetRequiredService(handlerGenericInterface));

                var apiHandlerTypeWithIHandlerMarker = GetMarkerApiHandlerType(handlerGenericInterface);
                var apiHandlerInterfaceWithIHandlerMarker =
                    ApiMarkerHandlerInterface.MakeGenericType(tRequest, tResponse, iHandlerInterface);

                _serviceCollection.AddScoped(apiHandlerInterfaceWithIHandlerMarker, apiHandlerTypeWithIHandlerMarker);
                DecorateApiHandler(apiHandlerInterfaceWithIHandlerMarker, tRequest, tResponse,iHandlerInterface);

                concreteApiHandler = apiHandlerInterfaceWithIHandlerMarker;
            }

            var apiHandlerInterface = ApiHandlerInterface.MakeGenericType(tRequest, tResponse);
            _serviceCollection.AddScoped(apiHandlerInterface, y => y.GetRequiredService(concreteApiHandler));
        }

        private void DecorateApiHandler(Type apiHandlerInterfaceWithMarker, Type tRequest, Type tResponse, Type markerInterface)
        {
            foreach (var genericDecorator in _genericDecorators)
            {
                var descriptor = _serviceCollection.Single(x => x.ServiceType == apiHandlerInterfaceWithMarker);
                var arguments = genericDecorator.GetGenericArguments();

                Type decorator = arguments.Length == 3 
                    ? genericDecorator.MakeGenericType(tRequest, tResponse, markerInterface) 
                    : genericDecorator.MakeGenericType(tRequest, tResponse);

                var objectFactory = ActivatorUtilities.CreateFactory(decorator, new[] {apiHandlerInterfaceWithMarker});

                _serviceCollection.Replace(ServiceDescriptor.Describe(
                    apiHandlerInterfaceWithMarker,
                    x => objectFactory.Invoke(x, new[] {CreateInstance(x, descriptor)}),
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

            // ReSharper disable once AssignNullToNotNullAttribute
            return ActivatorUtilities.GetServiceOrCreateInstance(services, descriptor.ImplementationType);
        }
    }
}
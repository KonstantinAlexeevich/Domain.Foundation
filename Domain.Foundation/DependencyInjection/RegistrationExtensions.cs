using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Domain.Foundation.Api;
using Domain.Foundation.Core;
using Domain.Foundation.CQRS;
using Microsoft.Extensions.DependencyInjection;

namespace Domain.Foundation.DependencyInjection
{
    public static class RegistrationExtensions
    {
        private static readonly Type QueryHandlerInterface = typeof(IQueryHandler<,>);
        private static readonly Type ApiQueryHandlerType = typeof(ApiQueryHandler<,,>);

        private static readonly Type CommandHandlerInterface = typeof(ICommandHandler<,>);
        private static readonly Type ApiCommandHandlerType = typeof(ApiCommandHandler<,,>);

        private static readonly Type AggregateCommandHandlerInterface = typeof(IAggregateCommandHandler<,,,>);
        private static readonly Type ApiAggregateCommandHandlerType = typeof(ApiAggregateCommandHandler<,,,,>);

        public static IServiceCollection AddDomainFoundation(this IServiceCollection serviceCollection, Action<IRegistrationOptions> configurationAction)
        {
            RegistrationOptions options = new RegistrationOptions();

            configurationAction?.Invoke(options);

            serviceCollection
                .AddAggregateFactory()
                .AddQueryHandlers(options)
                .AddCommandHandlers(options)
                .AddAggregateCommandHandlers(options);
            
            return serviceCollection;
        }

        private static IServiceCollection AddAggregateFactory(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IAggregateFactory, AggregateFactory>();
            return serviceCollection;
        }

        private static IServiceCollection AddQueryHandlers(this IServiceCollection serviceCollection, RegistrationOptions options)
        {
            options
                .GetAssemblies()
                .GetTypes()
                .WhereImplementsGenericInterface(QueryHandlerInterface)
                .ToList()
                .ForEach(handlerType =>
                {
                    var genericInterface = handlerType.GetGenericType(QueryHandlerInterface);

                    var genericArguments = genericInterface.GetGenericArguments();
                    var tRequest = genericArguments[0];
                    var tResponse = genericArguments[1];

                    Type GetApiHandlerType(Type markerInterface)
                    {
                        return ApiQueryHandlerType.MakeGenericType(tRequest, tResponse, markerInterface);
                    }

                    var helper = new RegistrationHelper(QueryHandlerInterface, tRequest, tResponse, GetApiHandlerType, options.GetApiHandlerDecorators());
                    helper.Register(serviceCollection, handlerType);
                });

            return serviceCollection;
        }

        private static IServiceCollection AddCommandHandlers(this IServiceCollection serviceCollection, RegistrationOptions options)
        {
            options
                .GetAssemblies()
                .GetTypes()
                .WhereImplementsGenericInterface(CommandHandlerInterface)
                .ToList()
                .ForEach(handlerType =>
                {
                    var genericInterface = handlerType.GetGenericType(CommandHandlerInterface);

                    var genericArguments = genericInterface.GetGenericArguments();
                    var tRequest = genericArguments[0];
                    var tResponse = genericArguments[1];

                    Type GetApiHandlerType(Type markerInterface)
                    {
                        return ApiCommandHandlerType.MakeGenericType(tRequest, tResponse, markerInterface);
                    }

                    var helper = new RegistrationHelper(CommandHandlerInterface, tRequest, tResponse, GetApiHandlerType, options.GetApiHandlerDecorators());
                    helper.Register(serviceCollection, handlerType);
                });

            return serviceCollection;
        }

        private static IServiceCollection AddAggregateCommandHandlers(this IServiceCollection serviceCollection, RegistrationOptions options)
        {
            options
                .GetAssemblies()
                .GetTypes()
                .WhereImplementsGenericInterface(AggregateCommandHandlerInterface)
                .ToList()
                .ForEach(handlerType =>
                {
                    var genericInterface = handlerType.GetGenericType(AggregateCommandHandlerInterface);

                    var genericArguments = genericInterface.GetGenericArguments();
                    var tAggregate = genericArguments[0];
                    var tIdentity = genericArguments[1];
                    var tRequest = genericArguments[2];
                    var tResponse = genericArguments[3];

                    Type GetApiHandlerType(Type markerInterface)
                    {
                        return ApiAggregateCommandHandlerType.MakeGenericType(tAggregate, tIdentity, tRequest, tResponse, markerInterface);
                    }

                    var helper = new RegistrationHelper(AggregateCommandHandlerInterface, tRequest, tResponse, GetApiHandlerType, options.GetApiHandlerDecorators());
                    helper.Register(serviceCollection, handlerType);
                });

            return serviceCollection;
        }

        internal static IEnumerable<Type> GetMarkerInterfaces(this TypeInfo x, Type handlerType)
        {
            return x.ImplementedInterfaces
                .Where(y => !y.IsGenericType &&
                    y.GetTypeInfo().ImplementedInterfaces
                        .Any(z => z.IsGenericType && z.GetGenericTypeDefinition() == handlerType));
        }

        internal static Type GetGenericType(this Type x, Type handlerType)
        {
            List<Type> types = new List<Type>();
            types.AddRange(x.GetTypeInfo().ImplementedInterfaces);
            types.Add(x);
            
            return types.Single(y => y.IsGenericType && y.GetGenericTypeDefinition() == handlerType);
        }

        static IEnumerable<TypeInfo> WhereImplementsGenericInterface(this IEnumerable<TypeInfo> types,
            Type type)
        {
            return types.Where(x => x.IsClass &&
                                    !x.IsAbstract &&
                                    !x.IsGenericType &&
                                    x.AsType().ImplementsGenericInterface(type));
        }

        static IEnumerable<TypeInfo> GetTypes(this IEnumerable<Assembly> assemblies)
        {
            return assemblies
                .Where(assembly => !assembly.IsDynamic)
                .Distinct()
                .SelectMany(assembly => assembly.DefinedTypes);
        }

        internal static bool ImplementsGenericInterface(this Type type, Type interfaceType)
        {
            return type.IsGenericType(interfaceType) || type.GetTypeInfo().ImplementedInterfaces
                .Any(@interface => @interface.IsGenericType(interfaceType));
        }

        internal static bool IsGenericType(this Type type, Type genericType)
        {
            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == genericType;
        }
    }
}
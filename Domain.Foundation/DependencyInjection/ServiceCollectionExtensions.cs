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
    public static class ServiceCollectionExtensions
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
            var helper = new RegistrationHelper(
                serviceCollection,
                QueryHandlerInterface, 
                options.GetApiHandlerDecorators());
            
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

                    helper.WithGetApiHandler(GetApiHandlerType)
                        .Register(handlerType, tRequest, tResponse);
                });

            return serviceCollection;
        }

        private static IServiceCollection AddCommandHandlers(this IServiceCollection serviceCollection, RegistrationOptions options)
        {
            
            var helper = new RegistrationHelper(
                serviceCollection,
                CommandHandlerInterface, 
                options.GetApiHandlerDecorators());
            
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

                    helper.WithGetApiHandler(GetApiHandlerType)
                        .Register(handlerType, tRequest, tResponse);
                });

            return serviceCollection;
        }

        private static IServiceCollection AddAggregateCommandHandlers(this IServiceCollection serviceCollection, RegistrationOptions options)
        {
            var helper = new RegistrationHelper(
                serviceCollection,
                AggregateCommandHandlerInterface, 
                options.GetApiHandlerDecorators());
            
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
                        return ApiAggregateCommandHandlerType.MakeGenericType(
                            tAggregate, tIdentity, tRequest, tResponse, markerInterface);
                    }

                    helper.WithGetApiHandler(GetApiHandlerType)
                        .Register(handlerType, tRequest, tResponse);
                });

            return serviceCollection;
        }
    }
}
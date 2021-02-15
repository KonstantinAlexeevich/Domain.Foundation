using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Domain.Foundation.Tactical;

namespace Domain.Foundation.Core
{
    public class AggregateFactory : IAggregateFactory
    {
        private static readonly ConcurrentDictionary<Type, AggregateConstruction> AggregateConstructions =
            new();

        private readonly IServiceProvider _serviceProvider;

        public AggregateFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<TAggregate> CreateAggregateInstanceAsync<TAggregate, TIdentity>(TIdentity id)
            where TAggregate : IAggregate<TIdentity>
        {
            var aggregateConstruction = AggregateConstructions.GetOrAdd(
                typeof(TAggregate),
                _ => CreateAggregateConstruction<TAggregate, TIdentity>());

            return aggregateConstruction.CreateInstance<TAggregate>(id, _serviceProvider);
        }

        private static AggregateConstruction CreateAggregateConstruction<TAggregate, TIdentity>()
        {
            var typeInfo = typeof(TAggregate).GetTypeInfo();
            var identityType = typeof(TIdentity);

            var factoryMethod = typeInfo
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(x => x.ReturnType == typeof(Task<TAggregate>))
                .SingleOrDefault(x => x.GetCustomAttributesData()
                    .Any(y => y.AttributeType == typeof(AggregateFactoryAttribute)));

            if (factoryMethod != null)
                return new AggregateConstruction(
                    factoryMethod.GetParameters(),
                    factoryMethod,
                    identityType);

            var constructorInfos = typeof(TAggregate)
                .GetTypeInfo()
                .GetConstructors()
                .ToList();

            ConstructorInfo constructorInfo;

            if (constructorInfos.Count != 1)
            {
                constructorInfo = constructorInfos
                    .SingleOrDefault(x => x.GetCustomAttributesData()
                        .Any(y => y.AttributeType == typeof(AggregateFactoryAttribute)));

                if (constructorInfo == null)
                    throw new ArgumentException(
                        $"Cant resolve from multiple constructors of '{typeof(TAggregate).FullName}' doesn't have just one constructor, or constructor with AggregateFactoryAttribute, or static factory method with return Task<TAggregate> and AggregateFactoryAttribute ");
            }
            else
            {
                constructorInfo = constructorInfos.Single();
            }

            return new AggregateConstruction(
                constructorInfo.GetParameters(),
                constructorInfo,
                identityType);
        }

        private class AggregateConstruction
        {
            private readonly ConstructorInfo _constructorInfo;

            private readonly MethodInfo _factoryMethod;
            private readonly Type _identityType;
            private readonly IReadOnlyCollection<ParameterInfo> _parameterInfos;

            public AggregateConstruction(
                IReadOnlyCollection<ParameterInfo> parameterInfos,
                MethodInfo factoryMethod,
                Type identityType)
            {
                _parameterInfos = parameterInfos;
                _factoryMethod = factoryMethod;
                _identityType = identityType;
            }

            public AggregateConstruction(
                IReadOnlyCollection<ParameterInfo> parameterInfos,
                ConstructorInfo constructorInfo,
                Type identityType)
            {
                _parameterInfos = parameterInfos;
                _constructorInfo = constructorInfo;
                _identityType = identityType;
            }

            public Task<T> CreateInstance<T>(object identity, IServiceProvider resolver) where T : IAggregate
            {
                var parameters = CreateParameters<T>(identity, resolver);

                if (_factoryMethod != null)
                    return (Task<T>) _factoryMethod.Invoke(null, parameters);

                if (_constructorInfo != null)
                    return Task.FromResult((T) _constructorInfo.Invoke(parameters));

                throw new Exception("No methods for create");
            }

            private object[] CreateParameters<T>(object identity, IServiceProvider resolver) where T : IAggregate
            {
                var parameters = new object[_parameterInfos.Count];
                foreach (var parameterInfo in _parameterInfos)
                    if (parameterInfo.ParameterType == _identityType)
                    {
                        parameters[parameterInfo.Position] = identity;
                    }
                    else
                    {
                        var service = resolver.GetService(parameterInfo.ParameterType);

                        if (service == null)
                            throw new ArgumentException($"Unable to resolve {parameterInfo.ParameterType}");

                        parameters[parameterInfo.Position] = resolver.GetService(parameterInfo.ParameterType);
                    }

                return parameters;
            }
        }
    }
}
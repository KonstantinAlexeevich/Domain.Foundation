using System;
using System.Collections.Generic;
using System.Reflection;
using Domain.Foundation.Api;

namespace Domain.Foundation.DependencyInjection
{
    public interface IRegistrationOptions
    {
        IRegistrationOptions DecorateApiHandler(Type decoratedType);
        IRegistrationOptions AddAssemblies(params Assembly[] assemblies);
    }
    
    public class RegistrationOptions: IRegistrationOptions
    {
        private readonly List<Type> _decorators = new List<Type>();
        private readonly List<Assembly> _assemblies = new List<Assembly>();
        
        public IRegistrationOptions DecorateApiHandler(Type decoratedType)
        {
            if (!decoratedType.ImplementsGenericInterface(typeof(IApiHandler<,>)))
                throw new ArgumentException("Type must be implements IApiHandler<,>");

            _decorators.Add(decoratedType);
            return this;
        }

        public IRegistrationOptions AddAssemblies(params Assembly[] assemblies)
        {
            _assemblies.AddRange(assemblies);
            return this;
        }

        public List<Type> GetApiHandlerDecorators()
        {
            return _decorators;
        }
        
        public List<Assembly> GetAssemblies()
        {
            return _assemblies;
        }
    }
}
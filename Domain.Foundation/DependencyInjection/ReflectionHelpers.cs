using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Domain.Foundation.DependencyInjection
{
    public static class ReflectionHelpers
    {
        
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

        internal static IEnumerable<TypeInfo> WhereImplementsGenericInterface(this IEnumerable<TypeInfo> types,
            Type type)
        {
            return types.Where(x => x.IsClass &&
                                    !x.IsAbstract &&
                                    !x.IsGenericType &&
                                    x.AsType().ImplementsGenericInterface(type));
        }

        internal static IEnumerable<TypeInfo> GetTypes(this IEnumerable<Assembly> assemblies)
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
using System;

namespace Domain.Foundation.Core
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
    public class AggregateFactoryAttribute : Attribute
    {
    }
}
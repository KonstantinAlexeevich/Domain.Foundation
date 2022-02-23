using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Foundation.Tactical
{
    public interface IAggregate : IRestoredFrom<Unit>, IStoredTo<Unit>
    {
        Task IRestoredFrom<Unit>.Restore(Unit snapshot) => Task.CompletedTask;
        Task<Unit> IStoredTo<Unit>.Store() => Task.FromResult<Unit>(Unit.Value);
    }
    
    public interface IAggregate<TIdentity> : IAggregate 
    {
    }
}
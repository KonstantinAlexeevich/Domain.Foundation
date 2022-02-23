using System.Collections.Generic;
using Domain.Foundation.Events;

namespace Domain.Foundation.Tactical
{
    public interface IEventsAggregate<TIdentity, TEvent> : 
        IAggregate<TIdentity>, 
        IRestoredFrom<IEnumerable<TEvent>> ,
        IStoredTo<IEnumerable<TEvent>> where TEvent : IEvent
    {
        IReadOnlyCollection<TEvent> Changes { get; }
        int Version { get; }
        void ClearChanges();
        string GetId();
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Foundation.Events;

namespace Domain.Foundation.Tactical
{
    public abstract class EventsAggregate<TIdentity, TEvent> : IEventsAggregate<TIdentity, TEvent> 
        where TEvent : IEvent
    {
        protected void Emit(TEvent evt) {
            _changes.Add(evt);
            Apply(evt);
        }

        public Task Restore(IEnumerable<TEvent> events)
        {
            foreach (var @event in events) {
                _existing.Add(@event);
                Apply(@event);
                Version++;
            }
            
            return Task.CompletedTask;
        }

        protected abstract void Apply(TEvent evt);

        public IReadOnlyCollection<TEvent> Changes => _changes.AsReadOnly();
        protected IReadOnlyCollection<TEvent> Existing => _existing.AsReadOnly();
        public void ClearChanges() => _changes.Clear();
        public abstract string GetId();
        public int Version { get; private set; } = -1;

        readonly List<TEvent> _existing = new();
        readonly List<TEvent> _changes = new();
        public Task<IEnumerable<TEvent>> Store() => Task.FromResult(Changes.AsEnumerable());
    }
}
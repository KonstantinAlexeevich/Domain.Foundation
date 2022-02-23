using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Foundation.Events
{
    public class DomainEventBus : IDomainEventBus, IDomainEventStore
    {
        private readonly List<IEvent> _events = new List<IEvent>();
        
        public void Publish(IEvent @event)
        {
            _events.Add(@event);
        }

        public Task<ICollection<IEvent>> GetUnhandledEvents()
        {
            ICollection<IEvent> events = _events;
            return Task.FromResult(events);
        }
    }
}
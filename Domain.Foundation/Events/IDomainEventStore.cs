using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Foundation.Events
{
    public interface IDomainEventStore
    {
        Task<ICollection<IEvent>> GetUnhandledEvents();
    }
}
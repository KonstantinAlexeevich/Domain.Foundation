using System;

namespace Domain.Foundation.Events
{
    public class EventBase : IEvent
    {
        public DateTime OccuredOn { get; init; } = DateTime.Now;
    }
}
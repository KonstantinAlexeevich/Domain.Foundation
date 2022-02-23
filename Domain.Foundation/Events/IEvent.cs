using System;

namespace Domain.Foundation.Events
{
    public interface IEvent
    {
        public DateTime OccuredOn { get; init; }
    }
}
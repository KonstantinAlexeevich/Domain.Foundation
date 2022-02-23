using Domain.Foundation.Events;

namespace Domain.Foundation.Tactical
{
    public interface IApply<in TEvent> where TEvent : IEvent
    {
        protected void Apply(TEvent evt);
    }
}
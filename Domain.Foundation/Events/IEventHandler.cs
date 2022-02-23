using System.Threading.Tasks;

namespace Domain.Foundation.Events
{

    public interface IEventAggregateHandler<in TAggregate, TIdentity, in TEvent> : IEventHandler
        where TEvent : IEvent
    {
        Task HandleEvent(TAggregate aggregate, TEvent @event);
    }
    
    public interface IEventHandler<in TEvent> : IEventHandler
        where TEvent : IEvent
    {
        Task HandleEvent(TEvent @event);
    }
    
    public interface IEventHandler
    {
    }
}
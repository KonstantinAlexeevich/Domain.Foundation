namespace Domain.Foundation.Events
{
    public interface IDomainEventBus
    {
        void Publish(IEvent @event);
    }
}
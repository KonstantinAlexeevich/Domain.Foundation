namespace Domain.Foundation.CQRS
{
    public interface ICommand<TIdentity> : ICommand
    {
        public TIdentity AggregateId { get; init; }
    }

    public interface ICommand
    {
    }
}
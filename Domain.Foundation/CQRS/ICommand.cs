namespace Domain.Foundation.CQRS
{
    public interface ICommand<TIdentity> : ICommand
    {
        public TIdentity AggregateId { get; set; }
    }

    public interface ICommand
    {
    }
}
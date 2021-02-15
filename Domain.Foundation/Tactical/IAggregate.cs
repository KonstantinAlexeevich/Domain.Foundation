using System.Threading.Tasks;

namespace Domain.Foundation.Tactical
{
    public interface IAggregate
    {
        Task LoadAsync() => Task.CompletedTask;
    }
    
    public interface IAggregate<TIdentity> : IAggregate 
    {
    }
}
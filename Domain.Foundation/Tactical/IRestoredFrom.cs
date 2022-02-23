using System.Threading.Tasks;

namespace Domain.Foundation.Tactical
{
    public interface IRestoredFrom<T>
    {
        public Task Restore(T snapshot);
    }
    
    public interface IStoredTo<T>
    {
        public Task<T> Store();
    }
    
}
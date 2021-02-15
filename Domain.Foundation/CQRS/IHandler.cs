namespace Domain.Foundation.CQRS
{
    public interface IHandler<in TRequest, out TResponse>
    {
    }
}
using System.Linq;
using Domain.Foundation.SourceGenerator.Tests.TestAggregate;
using Domain.Foundation.Tactical;

namespace Domain.Foundation.Tests.TestAggregate
{
    public partial class Order : IApplyOrderEvents
    {
        protected override void Apply(IOrderEvents evt) => (this as IApplyOrderEvents).ApplyEvent(evt);

        void IApply<IOrderEvents.OrderItemAdded>.Apply(IOrderEvents.OrderItemAdded evt)
        {
            _orderItems.Add(new OrderItem
            {
                ProductId = evt.ProductId,
                Count = evt.Count
            });
        }

        void IApply<IOrderEvents.OrderItemRemoved>.Apply(IOrderEvents.OrderItemRemoved evt)
        {
            var removed = Enumerable.Single<OrderItem>(_orderItems, x => x.ProductId == evt.ProductId);
            _orderItems.Remove(removed);
        }
    }
}
using System;
using System.Collections.Generic;
using Domain.Foundation.SourceGenerator.Tests.TestAggregate;
using Domain.Foundation.Tactical;
using static Domain.Foundation.Tests.TestAggregate.IOrderEvents;

namespace Domain.Foundation.Tests.TestAggregate
{
    public partial class Order : EventsAggregate<string, IOrderEvents>
    {
        private readonly string _orderId;
        private readonly List<OrderItem> _orderItems = new List<OrderItem>();
        public override string GetId() => _orderId;
        
        public Order(string orderId)
        {
            _orderId = orderId ?? Guid.NewGuid().ToString();
        }

        public void AddItem(OrderItem orderItem)
        {
            Emit(new OrderItemAdded
            {
                OrderId = _orderId,
                ProductId = orderItem.ProductId,
                Count = orderItem.Count
            });
        }
    }
    
}
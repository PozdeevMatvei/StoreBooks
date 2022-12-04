using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Memory
{
    public class MockOrderRepository : IOrderRepository
    {
        private readonly List<Order> _orders = new();
        public Order Create()
        {
            var orderDto = Order.DtoFactory.Create();
            orderDto.Id = _orders.Count + 1;
            var order = new Order(orderDto);

            _orders.Add(order);

            return order;
        }

        public Order GetById(int orderId)
        {
            return _orders.Single(order => order.OrderId == orderId);
        }

        public void Update(Order order)
        {
            ;
        }
    }
}

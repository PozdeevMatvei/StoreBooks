using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store
{
    public class Order
    {
        public int OrderId { get; }
        public OrderItemCollection Items { get; }
        public string? CellPhone { get; set; }
        public int TotalCount => Items.Sum(item => item.Count);
        public decimal TotalPrice => Items.Sum(item => item.Price * item.Count)
                                                          + (Delivery?.DeliveryPrice ?? 0m);
        public OrderDelivery? Delivery { get; set; }
        public OrderPayment? Payment { get; set; }

        public Order(int orderId, IEnumerable<OrderItem> items)
        {
            OrderId = orderId;
            Items = new OrderItemCollection(items);
        }
    }
}

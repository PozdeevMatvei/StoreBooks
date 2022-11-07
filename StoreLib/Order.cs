using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store
{
    public class Order
    {
        private readonly List<OrderItem> _orderItems;

        public int OrderId { get; }
        public IReadOnlyCollection<OrderItem> Items => _orderItems;
        public int TotalCount => _orderItems.Sum(item => item.Count);
        public decimal TotalPrice => _orderItems.Sum(item => item.Price * item.Count);
        public Order(int orderId, IEnumerable<OrderItem>? items)
        {
            if(items == null)
                throw new ArgumentNullException(nameof(items));

            OrderId = orderId;
            _orderItems = new List<OrderItem>(items);
        }

        public void AddItem(Book? book, int count)
        {
            if (book == null)
                throw new ArgumentNullException(nameof(book));

            var orderItem = _orderItems.FirstOrDefault(item => item.BookId == book.BookId);

            if(orderItem == null)
            {
                _orderItems.Add(new OrderItem(book.BookId, count, book.Price));
            }
            else
            {
                _orderItems.Remove(orderItem);
                _orderItems.Add(new OrderItem(book.BookId, orderItem.Count + count, book.Price));
            }
        }
    }
}

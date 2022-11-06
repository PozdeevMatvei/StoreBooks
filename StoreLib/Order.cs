using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store
{
    public class Order
    {
        private List<OrderItem> _items;

        public int OrderId { get; }
        public IReadOnlyCollection<OrderItem> Items => _items;
        public int TotalCount => _items.Sum(item => item.Count);
        public decimal TotalPrice => _items.Sum(item => item.Price * item.Count);
        public Order(int orderId, IEnumerable<OrderItem> items)
        {
            if(items == null)
                throw new ArgumentNullException(nameof(items));

            OrderId = orderId;
            _items = new List<OrderItem>(items);
        }

        public void AddItem(Book book, int count)
        {
            if (book == null)
                throw new ArgumentNullException(nameof(book));

            var orderItem = _items.FirstOrDefault(item => item.BookId == book.BookId);

            if(orderItem == null)
            {
                _items.Add(new OrderItem(book.BookId, count, book.Price));
            }
            else
            {
                _items.Remove(orderItem);
                _items.Add(new OrderItem(book.BookId, orderItem.Count + count, book.Price));
            }
        }
    }
}

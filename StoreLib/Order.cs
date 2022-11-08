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

        public OrderItem GetOrderItem(int bookId)
        {
            int index = _orderItems.FindIndex(item => item.BookId == bookId);

            if (index == -1)
                throw new InvalidOperationException("Book not found.");

            return _orderItems[index];
        }
        public void AddOrUpdateOrderItem(Book? book, int count) //todo: разбить на 2 метода 
        {
            if (book == null)
                throw new ArgumentNullException(nameof(book));

            var index = _orderItems.FindIndex(item => item.BookId == book.BookId);

            if(index == -1)
                _orderItems.Add(new OrderItem(book.BookId, count, book.Price));
            else
                _orderItems[index].Count += count;
        }
        public void RemoveOrderItem(int bookId)
        {
            var index = _orderItems.FindIndex(item => item.BookId == bookId);

            if (index == -1)
                throw new InvalidOperationException("Order does not contain specified item.");

            _orderItems.RemoveAt(index);
        }
    }
}

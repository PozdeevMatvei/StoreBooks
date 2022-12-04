using Store.DTO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store
{
    public class OrderItemCollection : IReadOnlyCollection<OrderItem>
    {
        private readonly OrderDto _orderDto;
        private readonly List<OrderItem> _items;

        public OrderItemCollection(OrderDto orderDto)
        {
            if(orderDto == null)
                throw new ArgumentNullException(nameof(orderDto));

            _orderDto = orderDto;
            _items = orderDto.Items
                             .Select(OrderItem.Mapper.Map)
                             .ToList();
        }

        public int Count => _items.Count;

        public bool TryGet(int bookId, out OrderItem? orderItem)
        {
            var index = _items.FindIndex(item => item.BookId == bookId);
            if(index == -1)
            {
                orderItem = null;
                return false;
            }

            orderItem = _items[index];
            return true;
        }
        public OrderItem Get(int bookId)
        {
            if (TryGet(bookId, out OrderItem? orderItem))
                return orderItem!;

            throw new InvalidOperationException("Book not found.");
        }
        public OrderItem Add(int bookId, decimal price, int count)
        {
            if (TryGet(bookId, out OrderItem? orderItem))
                throw new InvalidOperationException("Book alredy exists.");

            if (count < 1)
                throw new InvalidOperationException("Too few books to add.");

            var orderItemDto = OrderItem.DtoFactory.Create(_orderDto, bookId, price, count);
            _orderDto.Items.Add(orderItemDto);

            orderItem = new OrderItem(orderItemDto);
            _items.Add(orderItem);

            return orderItem;
        }
        public OrderItem Add(OrderItem orderItem)
        {
            if (TryGet(orderItem.BookId, out _))
                throw new InvalidOperationException("Book alredy exists.");
            if(orderItem.Count < 1 )
                throw new InvalidOperationException("Too few books to add.");

            var orderItemDto = OrderItem.Mapper.Map(orderItem);

            _orderDto.Items.Add(orderItemDto);
            
            _items.Add(orderItem);

            return orderItem;
        }
        public void Remove(int bookId)
        {
            var index = _items.FindIndex(item => item.BookId == bookId);
            if (index == -1)
                throw new InvalidOperationException("Can not remove this book. Her not exists in the order.");

            _orderDto.Items.RemoveAt(index);
            _items.RemoveAt(index);
        }

        public IEnumerator<OrderItem> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => (_items as IEnumerable).GetEnumerator();
        
    }
}

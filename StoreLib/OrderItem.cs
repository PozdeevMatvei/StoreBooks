using Store.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store
{
    public class OrderItem
    {
        private readonly OrderItemDto _dto;
        public int BookId  => _dto.BookId;
        public int Count 
        {
            get { return _dto.Count; }
            set
            {
                ThrowIfInvalidCount(value);
                _dto.Count = value;
            }

        }
        public decimal Price
        {
            get => _dto.Price; 
            set => _dto.Price = value; 
        }

        public OrderItem(OrderItemDto dto)
        {
            _dto = dto;
        }

        private static void ThrowIfInvalidCount(int count)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException($"{nameof(count)} by zero or negative number.");
        }

        public static class DtoFactory
        {
            public static OrderItemDto Create(OrderDto order, int bookId, decimal price, int count)
            {
                if(order == null)
                    throw new ArgumentNullException(nameof(order));

                ThrowIfInvalidCount(count);

                return new OrderItemDto
                {
                    BookId = bookId,
                    Price = price,
                    Count = count,
                    Order = order
                };
            }
        }

        public static class Mapper
        {
            public static OrderItem Map(OrderItemDto orderItemDto) => new OrderItem(orderItemDto);
            public static OrderItemDto Map(OrderItem orderItem) => orderItem._dto;

        }
    }
}

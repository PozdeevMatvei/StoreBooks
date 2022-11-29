using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Tests
{
    public class OrderItemTests
    {
        private OrderItem CreateTestOrderItem(int bookId = 1, decimal price = 10m, int count = 1)
        {
            var orderDto = Order.Factory.Create();
            var orderItemDto = OrderItem.DtoFactory.Create(orderDto, bookId, price, count);

            return new OrderItem(orderItemDto);
        }
        [Fact]
        public void OrderItem_WithCountZero_ThrowArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                CreateTestOrderItem(1, 0m, 0);
            });
        }
        [Fact]
        public void OrderItem_WithCountNegativeNumber_ThrowArgumentOutOfRangeexception()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                CreateTestOrderItem(1, 0m,-1);
            });
        }
        [Fact]
        public void OrderItem_WithCountPositiveNumber_SetsCount()
        {
            var item = CreateTestOrderItem(1, 5m, 2);
            Assert.Equal(1, item.BookId);
            Assert.Equal(2, item.Count);
            Assert.Equal(5m, item.Price);
        }

        [Fact]
        public void Count_WithNegativeValue_ThrowArgumentOutOfRangeException()
        {
            var orderItem = CreateTestOrderItem(1, 0m, 2);
            Assert.Throws<ArgumentOutOfRangeException>(() => orderItem.Count = -1);
        }
        [Fact]
        public void Count_WithZeroValue_ThrowArgumentOutOfRangeException()
        {
            var orderItem = CreateTestOrderItem(1, 0m, 2);
            Assert.Throws<ArgumentOutOfRangeException>(() => orderItem.Count = 0);
        }
        [Fact]
        public void Count_WithPositiveValue_SetValue()
        {
            var orderItem = CreateTestOrderItem(1, 0m, 4);
            orderItem.Count = 2;
            
            Assert.Equal(2, orderItem.Count);

        }
    }
}

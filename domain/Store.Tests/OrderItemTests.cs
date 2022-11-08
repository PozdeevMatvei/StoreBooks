using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Tests
{
    public class OrderItemTests
    {
        [Fact]
        public void OrderItem_WithCountZero_ThrowArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new OrderItem(1, 0, 0m);
            });
        }
        [Fact]
        public void OrderItem_WithCountNegativeNumber_ThrowArgumentOutOfRangeexception()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new OrderItem(1, -1, 0m);
            });
        }
        [Fact]
        public void OrderItem_WithCountPositiveNumber_SetsCount()
        {
            var item = new OrderItem(1, 2, 5m);
            Assert.Equal(1, item.BookId);
            Assert.Equal(2, item.Count);
            Assert.Equal(5m, item.Price);
        }

        [Fact]
        public void Count_WithNegativeValue_ThrowArgumentOutOfRangeException()
        {
            var orderItem = new OrderItem(1, 2, 0m);
            Assert.Throws<ArgumentOutOfRangeException>(() => orderItem.Count = -1);
        }
        [Fact]
        public void Count_WithZeroValue_ThrowArgumentOutOfRangeException()
        {
            var orderItem = new OrderItem(1, 2, 0m);
            Assert.Throws<ArgumentOutOfRangeException>(() => orderItem.Count = 0);
        }
        [Fact]
        public void Count_WithPositiveValue_SetValue()
        {
            var orderItem = new OrderItem(1, 4, 0m);
            orderItem.Count = 2;
            Assert.Equal(2, orderItem.Count);

        }
    }
}

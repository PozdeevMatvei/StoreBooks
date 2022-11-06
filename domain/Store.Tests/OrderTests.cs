using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Tests
{
    public class OrderTests
    {
        [Fact]
        public void Order_WithNullItems_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new Order(1, null));
        }

        [Fact]
        public void TotalCount_WithEmptyItems_ReturnsZero()
        {
            var order = new Order(1, new OrderItem[0]);
            Assert.Equal(0, order.TotalCount);
        }
        [Fact]
        public void TotalPrice_WithEmptyItems_ReturnsZero()
        {
            var order = new Order(1, new OrderItem[0]);
            Assert.Equal(0, order.TotalPrice);
        }
        [Fact]
        public void TotalCount_WithNonEmptyItems_CalculatesTotalCount()
        {
            var order = new Order(1, new[]
            {
                new OrderItem(1, 5, 10m),
                new OrderItem(2, 10, 100m)
            });

            Assert.Equal(5 + 10, order.TotalCount);
        }
        [Fact]
        public void TotalPrice_WithNonEmptyItems_CalculatesTotalPrice()
        {
            var order = new Order(1, new[]
            {
                new OrderItem(1, 5, 10m),
                new OrderItem(2, 10, 100m)
            });

            Assert.Equal(5 * 10m + 10 * 100m, order.TotalPrice);
        }

        [Fact]
        public void AddItem_WithNullBook_ThrowsArgumentNullException()
        {
            var order = new Order(1, new OrderItem[]
            {
                new OrderItem(1, 2, 5m),
                new OrderItem(2, 4, 10m)
            });

            Assert.Throws<ArgumentNullException>(() => order.AddItem(null, 1));
        }
        [Fact]
        public void AddItem_WithBookIsOrderItems_CalculateCount()
        {
            var order = new Order(1, new OrderItem[]
            {
                new OrderItem(1, 2, 5m),
                new OrderItem(2, 4, 10m)
            });

            var bookIsOrderItems = new Book(1, "", "", "", "", 5m);

            order.AddItem(bookIsOrderItems, 2);
            var item = order.Items.FirstOrDefault(book => bookIsOrderItems.BookId == book.BookId);

            Assert.NotNull(item);
            Assert.Equal(bookIsOrderItems.BookId, item?.BookId);
            Assert.Equal(2 + 2, item?.Count);
            Assert.Equal(bookIsOrderItems.Price, item?.Price);
        }
    }
}

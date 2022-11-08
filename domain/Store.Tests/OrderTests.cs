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
        public void AddOrUpdateOrderItem_WithNull_ThrowsArgumentNullException()
        {
            var order = new Order(1, new OrderItem[]
            {
                new OrderItem(1, 2, 5m),
                new OrderItem(2, 4, 10m)
            });

            Assert.Throws<ArgumentNullException>(() => order.AddOrUpdateOrderItem(null, 1));
        }
        [Fact]
        public void AddOrUpdateOrderItem_WithExistingBook_UpdateCount()
        {
            var order = new Order(1, new OrderItem[]
            {
                new OrderItem(1, 2, 5m),
                new OrderItem(2, 4, 10m)
            });

            var existingBook = new Book(1, "", "", "", "", 5m);

            order.AddOrUpdateOrderItem(existingBook, 2);
            var orderItem = order.Items.FirstOrDefault(item => existingBook.BookId == item.BookId);

            Assert.NotNull(orderItem);
            Assert.Equal(existingBook.BookId, orderItem?.BookId);
            Assert.Equal(2 + 2, orderItem?.Count);
            Assert.Equal(existingBook.Price, orderItem?.Price);
        }
        [Fact]
        public void AddOrUpdateOrderItem_WithNonExistingBook_AddOrderItem()
        {
            var order = new Order(1, new OrderItem[]
            {
                new OrderItem(1, 2, 5m),
                new OrderItem(2, 4, 10m)
            });

            var notExistingBook = new Book(3, "", "", "", "", 15m);

            order.AddOrUpdateOrderItem(notExistingBook, 6);
            var orderItem = order.Items.FirstOrDefault(item => notExistingBook.BookId == item.BookId);

            Assert.Equal(notExistingBook.BookId, orderItem?.BookId);
            Assert.Equal(6, orderItem?.Count);
            Assert.Equal(notExistingBook.Price, orderItem?.Price);
        }

        [Fact]
        public void GetOrderItem_WithNonExistingBook_InvalidOperationException()
        {
            var order = new Order(1, new OrderItem[]
            {
                new OrderItem(1, 2, 5m),
                new OrderItem(2, 4, 10m)
            });
            var notExistingOrderItem = new OrderItem(3, 6, 15m);

            Assert.Throws<InvalidOperationException>(() => 
                order.GetOrderItem(notExistingOrderItem.BookId));
        }
        [Fact]
        public void GetOrderItem_WithExistingBook_GetOrderItem()
        {
            var existingOrderItem = new OrderItem(1, 2, 5m);

            var order = new Order(1, new OrderItem[]
            {
                existingOrderItem,
                new OrderItem(2, 4, 10m)
            });          

            var orderItem = order.GetOrderItem(existingOrderItem.BookId);

            Assert.Equal(existingOrderItem.BookId, orderItem?.BookId);
            Assert.Equal(existingOrderItem.Count, orderItem?.Count);
            Assert.Equal(existingOrderItem.Price, orderItem?.Price);

        }

        
        [Fact]
        public void RemoveOrderItem_WithNonExistingOrderItem_ThrowInvalidOperationException()
        {
            var order = new Order(1, new OrderItem[]
            {
                new OrderItem(1, 2, 5m),
                new OrderItem(2, 4, 10m)
            });

            var nonExistingOrderItem = new OrderItem(100, 2, 0m);

            Assert.Throws<InvalidOperationException>(() => 
                order.RemoveOrderItem(nonExistingOrderItem.BookId));
        }
        [Fact]
        public void RemoveOrderItem_WithExistingOrderItem_RemoveOrderItem()
        {
            var existingOrderItem = new OrderItem(1, 2, 5m);

            var order = new Order(1, new OrderItem[]
            {
                existingOrderItem,
                new OrderItem(2, 4, 10m)
            });

            order.RemoveOrderItem(existingOrderItem.BookId);
            Assert.Equal(1, order.Items.Count);
        }
    }
}

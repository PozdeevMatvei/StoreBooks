using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Tests
{
    public class OrderItemCollectionTests
    {
        [Fact]
        public void Get_WithNonExistingBook_InvalidOperationException()
        {
            var orderItemCollection = new OrderItemCollection(new OrderItem[]
            {
                new OrderItem(1, 5m, 2),
                new OrderItem(2, 10m, 4)
            });
            var notExistingOrderItem = new OrderItem(3, 15m, 6);

            Assert.Throws<InvalidOperationException>(() =>
                orderItemCollection.Get(notExistingOrderItem.BookId));
        }
        [Fact]
        public void Get_WithExistingBook_GetOrderItem()
        {
            var existingOrderItem = new OrderItem(1, 5m, 2);

            var orderItemCollection = new OrderItemCollection(new OrderItem[]
            {
                existingOrderItem,
                new OrderItem(2, 10m, 4)
            });

            var orderItem = orderItemCollection.Get(existingOrderItem.BookId);

            Assert.Equal(existingOrderItem.BookId, orderItem?.BookId);
            Assert.Equal(existingOrderItem.Count, orderItem?.Count);
            Assert.Equal(existingOrderItem.Price, orderItem?.Price);

        }


        [Fact]
        public void RemoveOrderItem_WithNonExistingOrderItem_ThrowInvalidOperationException()
        {
            var orderItemCollection = new OrderItemCollection(new OrderItem[]
            {
                new OrderItem(1, 5m, 2),
                new OrderItem(2, 10m, 4)
            });

            var nonExistingOrderItem = new OrderItem(100, 0m, 2);

            Assert.Throws<InvalidOperationException>(() =>
                orderItemCollection.Remove(nonExistingOrderItem.BookId));
        }
        [Fact]
        public void RemoveOrderItem_WithExistingOrderItem_RemoveOrderItem()
        {
            var existingOrderItem = new OrderItem(1, 5m, 2);

            var orderItemCollection = new OrderItemCollection(new OrderItem[]
            {
                existingOrderItem,
                new OrderItem(2, 10m, 4)
            });

            orderItemCollection.Remove(existingOrderItem.BookId);
            int actualCount = 1;
            int resultCount = orderItemCollection.Count;
            Assert.Equal(actualCount, resultCount);
        }
    }
}

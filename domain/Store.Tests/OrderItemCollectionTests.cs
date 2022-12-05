namespace Store.Tests
{
    public class OrderItemCollectionTests
    {
        private OrderItem CreateTestOrderItem(int bookId = 1, decimal price = 10m, int count = 1)
        {
            var orderDto = Order.DtoFactory.Create();
            var orderItemDto = OrderItem.DtoFactory.Create(orderDto, bookId, price, count);

            return new OrderItem(orderItemDto);
        }
        private OrderItemCollection CreateTestOrderItemCollection()
        {
            var orderDto = Order.DtoFactory.Create();
            return new OrderItemCollection(orderDto);
        }
        [Fact]
        public void Get_WithNonExistingBook_InvalidOperationException()
        {
            var orderItemCollection = CreateTestOrderItemCollection();
            orderItemCollection.Add(1, 5m, 4);

            var notExistingOrderItem = CreateTestOrderItem(2, 3m, 1);

            Assert.Throws<InvalidOperationException>(() =>
                orderItemCollection.Get(notExistingOrderItem.BookId));
        }
        [Fact]
        public void Get_WithExistingBook_GetOrderItem()
        {
            var existingOrderItem = CreateTestOrderItem(1, 5m, 1);

            var orderItemCollection = CreateTestOrderItemCollection();
            orderItemCollection.Add(existingOrderItem);

            var orderItem = orderItemCollection.Get(existingOrderItem.BookId);

            Assert.Equal(existingOrderItem.BookId, orderItem?.BookId);
            Assert.Equal(existingOrderItem.Count, orderItem?.Count);
            Assert.Equal(existingOrderItem.Price, orderItem?.Price);

        }


        [Fact]
        public void RemoveOrderItem_WithNonExistingOrderItem_ThrowInvalidOperationException()
        {
            var orderItemCollection = CreateTestOrderItemCollection();
            orderItemCollection.Add(1, 2m, 2);

            var nonExistingOrderItem = CreateTestOrderItem(100, 0m, 2);

            Assert.Throws<InvalidOperationException>(() =>
                orderItemCollection.Remove(nonExistingOrderItem.BookId));
        }
        [Fact]
        public void RemoveOrderItem_WithExistingOrderItem_RemoveOrderItem()
        {
            var existingOrderItem = CreateTestOrderItem(1, 5m, 2);

            var orderItemCollection = CreateTestOrderItemCollection();
            orderItemCollection.Add(existingOrderItem);

            orderItemCollection.Remove(existingOrderItem.BookId);
            int actualCount = 1;
            int resultCount = orderItemCollection.Count;
            Assert.Equal(actualCount, resultCount);
        }
    }
}

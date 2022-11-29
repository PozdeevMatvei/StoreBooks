using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Tests
{
    public class OrderTests
    {
        private Order CreateTestOrder()
        {
            var orderDto = Order.Factory.Create();
            return new Order(orderDto);
        }
        [Fact]
        public void Order_WithNullItems_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new Order(null));
        }

        [Fact]
        public void TotalCount_WithEmptyItems_ReturnsZero()
        {
            var order = CreateTestOrder();
            Assert.Equal(0, order.TotalCount);
        }
        [Fact]
        public void TotalPrice_WithEmptyItems_ReturnsZero()
        {
            var order = CreateTestOrder();
            Assert.Equal(0, order.TotalPrice);
        }
        [Fact]
        public void TotalCount_WithNonEmptyItems_CalculatesTotalCount()
        {
            var order = CreateTestOrder();
            order.Items.Add(1, 10m, 5);
            order.Items.Add(2, 100m, 10);

            Assert.Equal(5 + 10, order.TotalCount);
        }
        [Fact]
        public void TotalPrice_WithNonEmptyItems_CalculatesTotalPrice()
        {
            var order = CreateTestOrder();
            order.Items.Add(1, 10m, 5);
            order.Items.Add(2, 100m, 10);

            Assert.Equal(5 * 10m + 10 * 100m, order.TotalPrice);
        }
    
    }
}

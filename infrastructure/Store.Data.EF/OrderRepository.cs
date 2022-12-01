using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.DTO.EF
{
    public class OrderRepository : IOrderRepository
    {
        private readonly StoreDbContextFactory _dbContextFactory;
        public OrderRepository(StoreDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public Order Create()
        {
            var dbContext = _dbContextFactory.GetOrCreate(typeof(OrderRepository));
            var orderDto = Order.Factory.Create();

            dbContext.Orders.Add(orderDto);
            dbContext.SaveChanges();

            return Order.Mapper.Map(orderDto);
        }

        public Order GetById(int orderId)
        {
            var dbContext = _dbContextFactory.GetOrCreate(typeof(OrderRepository));
            var orderDto = dbContext.Orders
                                    .Include(order => order.Items)  
                                    .Single(order => order.OrderId == orderId);

            return Order.Mapper.Map(orderDto);
        }

        public void Update(Order order)
        {
            var dbContext = _dbContextFactory.GetOrCreate(typeof(OrderRepository));
            dbContext.SaveChanges();
        }
    }
}

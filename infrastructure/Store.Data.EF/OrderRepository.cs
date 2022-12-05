using Microsoft.EntityFrameworkCore;

namespace Store.DTO.EF
{
    public class OrderRepository : IOrderRepository
    {
        private readonly StoreDbContextFactory _dbContextFactory;
        public OrderRepository(StoreDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<Order> CreateAsync()
        {
            var dbContext = _dbContextFactory.GetOrCreate(typeof(OrderRepository));
            var orderDto = Order.DtoFactory.Create();

            dbContext.Orders.Add(orderDto);
            await dbContext.SaveChangesAsync();

            return Order.Mapper.Map(orderDto);
        }

        public async Task<Order> GetByIdAsync(int orderId)
        {
            var dbContext = _dbContextFactory.GetOrCreate(typeof(OrderRepository));
            var orderDto = await dbContext.Orders
                                    .Include(order => order.Items)
                                    .SingleAsync(order => order.Id == orderId);

            return Order.Mapper.Map(orderDto);
        }

        public async Task UpdateAsync(Order order)
        {
            var dbContext = _dbContextFactory.GetOrCreate(typeof(OrderRepository));
            await dbContext.SaveChangesAsync();
        }
    }
}

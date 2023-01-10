namespace Store
{
    public interface IOrderRepository
    {
        Task<Order> CreateAsync();
        Task<Order> GetByIdAsync(int orderId);
        Task UpdateAsync();
    }
}

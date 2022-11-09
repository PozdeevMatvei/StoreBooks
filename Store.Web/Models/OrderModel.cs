namespace Store.Web.Models
{
    public class OrderModel
    {
        public int OrderId { get; set; }
        public OrderItemModel[] OrderItems { get; set; } = Array.Empty<OrderItemModel>();
        public int TotalCount { get; set; }
        public decimal TotalPrice { get; set; }
        public Dictionary<string, string> Errors { get; set; } = new Dictionary<string, string>();
    }
}

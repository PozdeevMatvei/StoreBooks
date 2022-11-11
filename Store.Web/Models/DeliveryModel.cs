namespace Store.Web.Models
{
    public class DeliveryModel
    {
        public int OrderId { get; set; }
        public IDictionary<string, string>? Methods { get; set; }
    }
}

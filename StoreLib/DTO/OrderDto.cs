using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.DTO
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string? CellPhone { get; set; }
        public int TotalCount { get; set; }
        public decimal TotalPrice { get; set; }

        public string? DeliveryName { get; set; }
        public string? DeliveryDescription { get; set; }
        public decimal DeliveryPrice { get; set; }
        public Dictionary<string, string>? DeliveryParameters { get; set; }

        public string? PaymentName { get; set; }
        public string? PaymentDescription { get; set; }
        public string? IsCompletePaymentOrder { get; set; }
        public Dictionary<string, string>? PaymentParameters { get; set; }

        public IList<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
    }
}

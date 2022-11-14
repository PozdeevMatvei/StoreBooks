using Store.Contractors;
using Store.Web.Contractors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.YandexKassa
{
    public class YandexKassaPaymentService : IWebContractorService, IPaymentService
    {
        public string UniqueCode => "YandexKassa";
        public string Title => "Оплата банковской картой";
        public string GetUri => "/YandexKassa/";

        

        public Form CreateForm(Order order)
        {
            return new Form(UniqueCode, order.OrderId, 1, false, Array.Empty<Field>());
        }

        public OrderPayment GetOrderPayment(Form form)
        {
            string description = "Оплата банковской картой";
            return new OrderPayment(UniqueCode, description, new Dictionary<string, string>());
        }

        public Form MoveNextForm(int orderId, int step, IReadOnlyDictionary<string, string> values)
        {
            return new Form(UniqueCode, orderId, 2, true, Array.Empty<Field>());
        }
    }
}

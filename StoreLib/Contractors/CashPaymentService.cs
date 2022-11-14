using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Contractors
{
    public class CashPaymentService : IPaymentService
    {
        public string UniqueCode => "Cash";

        public string Title => "Оплата наличными";

        public Form CreateForm(Order order)
        {
            return new Form(UniqueCode, order.OrderId, 1, false, Array.Empty<Field>());
        }

        public OrderPayment GetOrderPayment(Form form)
        {
            if (form.UniqueCode != UniqueCode || !form.IsFinal)
                throw new InvalidOperationException("Invalid payment form");

            string description = "Оплата наличными";
            var emptyParameters = new Dictionary<string, string>();

            return new OrderPayment(UniqueCode, description, emptyParameters);
        }

        public Form MoveNextForm(int orderId, int step, IReadOnlyDictionary<string, string> values)
        {
            if(step != 1)
                throw new InvalidOperationException("Invalid cash step.");

            return new Form(UniqueCode, orderId, 2, true, Array.Empty<Field>());
        }
    }
}

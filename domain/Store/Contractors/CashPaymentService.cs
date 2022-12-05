namespace Store.Contractors
{
    public class CashPaymentService : IPaymentService
    {
        public string Name => "Cash";

        public string Title => "Оплата наличными";

        public Form FirstForm(Order order)
        {
            return Form.CreateFirst(Name)
                       .AddParameter("orderId", order.OrderId.ToString());
        }
        public Form NextForm(int step, IReadOnlyDictionary<string, string> parameters)
        {
            if (step != 1)
                throw new InvalidOperationException("Invalid cash step.");

            return Form.CreateLast(Name, step + 1, parameters);
        }

        public OrderPayment GetPayment(Form form)
        {
            if (form.ServiceName != Name || !form.IsFinal)
                throw new InvalidOperationException("Invalid payment form");

            string description = "Оплата наличными";

            return new OrderPayment(Name, description, form.Parameters);
        }
    }
}

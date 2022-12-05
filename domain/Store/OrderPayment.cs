namespace Store
{
    public class OrderPayment
    {
        public string Name { get; }
        public string? Description { get; }
        public string IsCompletePaymentOrder { get; }
        public IReadOnlyDictionary<string, string>? Parameters { get; }

        public OrderPayment(string name, string? description, IReadOnlyDictionary<string, string>? parameters)
        {
            Name = name;
            Description = description;
            Parameters = parameters;
            IsCompletePaymentOrder = SetIsCompletePaymentOrder(parameters);
        }

        private static string SetIsCompletePaymentOrder(IReadOnlyDictionary<string, string>? parameters)
        {
            if (parameters != null && parameters.ContainsKey("IsCompletedPaymentOrder"))
                return parameters["IsCompletedPaymentOrder"] == PaymentCompletionStatus.orderPaid.ToString() ? "Заказ оплачен." : "Заказ не оплачен.";

            return "Заказ не оплачен.";
        }
    }
}

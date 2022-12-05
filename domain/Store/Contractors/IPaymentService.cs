namespace Store.Contractors
{
    public interface IPaymentService
    {
        string Name { get; }
        string Title { get; }
        Form FirstForm(Order order);
        Form NextForm(int step, IReadOnlyDictionary<string, string> parameters);
        OrderPayment GetPayment(Form form);
    }
}

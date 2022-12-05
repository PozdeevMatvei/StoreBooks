namespace Store.YandexKassa.Areas.YandexKassa.Models
{
    public record class ExampleModel
    (
        int OrderId,
        string ReturnUri,
        PaymentCompletionStatus IsCompletedPaymentOrder = PaymentCompletionStatus.orderNotPaid
    );

}

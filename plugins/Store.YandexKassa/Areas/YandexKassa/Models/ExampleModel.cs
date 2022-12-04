using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.YandexKassa.Areas.YandexKassa.Models
{  
    public record class ExampleModel
    (
        int OrderId, 
        string ReturnUri, 
        PaymentCompletionStatus IsCompletedPaymentOrder = PaymentCompletionStatus.orderNotPaid
    );
    
}

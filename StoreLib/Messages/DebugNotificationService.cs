using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Messages
{
    public class DebugNotificationService : INotificationService
    {
        public void SendConfirmationCode(string cellPhone, int code)
        {
            Debug.WriteLine($"Cell phone: {cellPhone}, code: {code:0000}.");
        }

        public async Task SendConfirmationCodeAsync(string cellPhone, int code)
        {
            SendConfirmationCode(cellPhone, code);

            await Task.CompletedTask;
        }

        public void StartProcess(Order order)
        {
            Debug.WriteLine("Order ID {0}", order.OrderId);
            Debug.WriteLine("Delivery: {0}", (object?)order.Delivery?.Description);
            Debug.WriteLine("Payment: {0}", (object?)order.Payment?.Description);
        }

        public async Task StartProcessAsync(Order order)
        {
            StartProcess(order);

            await Task.CompletedTask;
        }
    }
}

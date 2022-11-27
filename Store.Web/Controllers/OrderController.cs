using Microsoft.AspNetCore.Mvc;
using Store.Messages;
using System.Text.RegularExpressions;
using Store.Contractors;
using Store.Memory;
using Store.Web.Contractors;
using Store.Web.App;

namespace Store.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly OrderService _orderService;
        private readonly IEnumerable<IDeliveryService> _deliveryServices;
        private readonly IEnumerable<IPaymentService> _paymentServices;
        private readonly IEnumerable<IWebContractorService> _webContractorServices;

        public OrderController(OrderService orderService,
                               IEnumerable<IDeliveryService> deliveryServices, 
                               IEnumerable<IPaymentService> paymentServices, 
                               IEnumerable<IWebContractorService> webContractorServices)
        {        
            _orderService = orderService;
            _deliveryServices = deliveryServices;
            _paymentServices = paymentServices;
            _webContractorServices = webContractorServices;
        }

        public IActionResult Index()
        {
            if (_orderService.TryGetModel(out OrderModel? model))
                return View(model);

            return View("Empty");
        }
        [HttpPost]
        public IActionResult AddOrderItem(int bookId, int count = 1)
        {
            _orderService.AddOrderItem(bookId, count);

            return RedirectToAction(nameof(BookController.Index), "Book", new { BookId = bookId});
        }
        [HttpPost]
        public IActionResult RemoveOrderItem(int bookId)
        {
            _orderService.RemoveOrderItem(bookId);

            return RedirectToAction(nameof(OrderController.Index));
        }
        [HttpPost]
        public IActionResult UpdateOrderItem(int bookId, int count = 1)
        {
            _orderService.UpdateOrderItem(bookId, count);

            return RedirectToAction(nameof(OrderController.Index));
        }

        public IActionResult AddBook(int bookId)
        {
            _orderService.AddBook(bookId);

            return RedirectToAction(nameof(OrderController.Index));
        }
        public IActionResult PutAwayBook(int bookId)
        {
            _orderService.PutAwayBook(bookId);
            return RedirectToAction(nameof(OrderController.Index));
        }

        [HttpPost]
        public IActionResult SendConfirmationCode(string cellPhone)
        {
            var orderModel = _orderService.SendConfirmation(cellPhone);

            return View("Confirmation", orderModel);
        }
        [HttpPost]
        public IActionResult ConfirmCellPhone(string cellPhone, int code)
        {
            var orderModel = _orderService.ConfirmCellPhone(cellPhone, code);

            if (orderModel.Errors.Count > 0)
                return View("Confirmation", orderModel);

            var deliveryMethods = _deliveryServices.ToDictionary(service => service.Name,
                                                                 service => service.Title);

            return View("DeliveryMethod", deliveryMethods);
        }


        [HttpPost]
        public IActionResult StartDelivery(string serviceName)
        {
            var deliveryService = _deliveryServices.Single(service => service.Name == serviceName);
            var order = _orderService.GetOrder();

            var form = deliveryService.FirstForm(order);

            var webContractorService = _webContractorServices.SingleOrDefault(service => service.Name == serviceName);
            if (webContractorService == null)
                return View("DeliveryStep", form);

            var returnUri = GetReturnUri(nameof(NextDelivery));
            var redirectUri = webContractorService.StartSession(form.Parameters, returnUri);

            return Redirect(redirectUri.ToString());
        }
        [HttpPost]
        public IActionResult NextDelivery(string serviceName, int step, Dictionary<string, string> parameters)
        {
            var deliveryService = _deliveryServices.Single(service => service.Name == serviceName);

            var form = deliveryService.NextForm(step, parameters);

            if (!form.IsFinal)
                return View("DeliveryStep", form);

            var delivery = deliveryService.GetDelivery(form);
            _orderService.SetDelivery(delivery);

            var paymentMethods = _paymentServices.ToDictionary(service => service.Name,
                                                               service => service.Title);

            return View("PaymentMethod", paymentMethods);
        }

        public IActionResult StartPayment(string serviceName)
        {
            var paymentService = _paymentServices.Single(service => service.Name == serviceName);
            var order = _orderService.GetOrder();
            var form = paymentService.FirstForm(order);

            var webContractorService = _webContractorServices
                                       .SingleOrDefault(service => service.Name == serviceName);

            if (webContractorService == null)
                return View("PaymentStep", form);

            var returnUri = GetReturnUri(nameof(NextPayment));
            var redirectUri = webContractorService.StartSession(form.Parameters, returnUri);

            return Redirect(redirectUri.ToString());
        }
        [HttpPost]
        public IActionResult NextPayment(string serviceName, int step, 
            Dictionary<string, string> parameters)
        {
            var paymentService = _paymentServices.Single(service => service.Name == serviceName);

            var form = paymentService.NextForm(step, parameters);

            if (!form.IsFinal)
                return View("PaymentStep", form);

            var payment = paymentService.GetPayment(form);
            var orderModel = _orderService.SetPayment(payment); 

            return View("Finish", orderModel);
        }
        
       
        private Uri GetReturnUri(string action)
        {
            var builder = new UriBuilder(Request.Scheme, Request.Host.Host)
            {
                Path = Url.Action(action),
                Query = null
            };

            if (Request.Host.Port != null)
                builder.Port = Request.Host.Port.Value;

            return builder.Uri;
        }

    }
}

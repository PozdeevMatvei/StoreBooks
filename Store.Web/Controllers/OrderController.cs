using Microsoft.AspNetCore.Mvc;
using Store.Contractors;
using Store.Web.App.Services;
using Store.Web.Contractors;

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

        public async Task<IActionResult> Index()
        {
            var (isGetModel, orderModel) = await _orderService.TryGetModelAsync();
            if (isGetModel)
                return View(orderModel);

            return View("Empty");
        }
        [HttpPost]
        public async Task<IActionResult> AddOrderItem(int bookId, int count = 1)
        {
            await _orderService.AddOrderItemAsync(bookId, count);

            return RedirectToAction(nameof(BookController.Index), "Book", new { BookId = bookId });
        }
        [HttpPost]
        public async Task<IActionResult> RemoveOrderItem(int bookId)
        {
            await _orderService.RemoveOrderItemAsync(bookId);

            return RedirectToAction(nameof(OrderController.Index));
        }
        [HttpPost]
        public async Task<IActionResult> UpdateOrderItem(int bookId, int count = 1)
        {
            await _orderService.UpdateOrderItemAsync(bookId, count);

            return RedirectToAction(nameof(OrderController.Index));
        }

        public async Task<IActionResult> AddBook(int bookId)
        {
            await _orderService.AddBookAsync(bookId);

            return RedirectToAction(nameof(OrderController.Index));
        }
        public async Task<IActionResult> PutAwayBook(int bookId)
        {
            await _orderService.PutAwayBookAsync(bookId);
            return RedirectToAction(nameof(OrderController.Index));
        }

        [HttpPost]
        public async Task<IActionResult> SendConfirmationCode(string cellPhone)
        {
            var orderModel = await _orderService.SendConfirmationAsync(cellPhone);

            if (orderModel.Errors.ContainsKey("cellPhone"))
                return View("Index", orderModel);

            return View("Confirmation", orderModel);
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmCellPhone(string cellPhone, int code)
        {
            var orderModel = await _orderService.ConfirmCellPhoneAsync(cellPhone, code);

            if (orderModel.Errors.Count > 0)
                return View("Confirmation", orderModel);

            var deliveryMethods = _deliveryServices.ToDictionary(service => service.Name,
                                                                 service => service.Title);

            return View("DeliveryMethod", deliveryMethods);
        }


        [HttpPost]
        public async Task<IActionResult> StartDelivery(string serviceName)
        {
            var deliveryService = _deliveryServices.Single(service => service.Name == serviceName);
            var order = await _orderService.GetOrderAsync();

            var form = deliveryService.FirstForm(order);

            var webContractorService = _webContractorServices.SingleOrDefault(service => service.Name == serviceName);
            if (webContractorService == null)
                return View("DeliveryStep", form);

            var returnUri = GetReturnUri(nameof(NextDelivery));
            var redirectUri = webContractorService.StartSession(form.Parameters, returnUri);

            return Redirect(redirectUri.ToString());
        }
        [HttpPost]
        public async Task<IActionResult> NextDelivery(string serviceName, int step, Dictionary<string, string> parameters)
        {
            var deliveryService = _deliveryServices.Single(service => service.Name == serviceName);

            var form = deliveryService.NextForm(step, parameters);

            if (!form.IsFinal)
                return View("DeliveryStep", form);

            var delivery = deliveryService.GetDelivery(form);
            await _orderService.SetDeliveryAsync(delivery);

            var paymentMethods = _paymentServices.ToDictionary(service => service.Name,
                                                               service => service.Title);

            return View("PaymentMethod", paymentMethods);
        }
        [HttpPost]
        public async Task<IActionResult> StartPayment(string serviceName)
        {
            var paymentService = _paymentServices.Single(service => service.Name == serviceName);
            var order = await _orderService.GetOrderAsync();
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
        public async Task<IActionResult> NextPayment(string serviceName, int step,
            Dictionary<string, string> parameters)
        {
            var paymentService = _paymentServices.Single(service => service.Name == serviceName);

            var form = paymentService.NextForm(step, parameters);

            if (!form.IsFinal)
                return View("PaymentStep", form);

            var payment = paymentService.GetPayment(form);
            var orderModel = await _orderService.SetPaymentAsync(payment);

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

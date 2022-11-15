using Microsoft.AspNetCore.Mvc;
using Store.Web.Models;
using Store.Messages;
using System.Text.RegularExpressions;
using Store.Contractors;
using Store.Memory;
using Store.Web.Contractors;

namespace Store.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IBookRepository _bookRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IEnumerable<IDeliveryService> _deliveryServices;
        private readonly IEnumerable<IPaymentService> _paymentServices;
        private readonly IEnumerable<IWebContractorService> _webContractorServices;
        private readonly INotificationService _notificationService;

        public OrderController(IBookRepository bookRepository, 
                               IOrderRepository orderRepository,
                               IEnumerable<IDeliveryService> deliveryServices, 
                               IEnumerable<IPaymentService> paymentServices, 
                               IEnumerable<IWebContractorService> webContractorServices,
                               INotificationService notificationService)
        {
            _bookRepository = bookRepository;
            _orderRepository = orderRepository;
            _deliveryServices = deliveryServices;
            _paymentServices = paymentServices;
            _webContractorServices = webContractorServices;
            _notificationService = notificationService;
        }

        public IActionResult Index()
        {
            if(HttpContext.Session.TryGetCart(out Cart? cart))
            {
                var order = _orderRepository.GetById(cart!.OrderId);
                var orderModel = Map(order);

                return View(orderModel);
            }
            return View("Empty");
        }
        [HttpPost]
        public IActionResult AddItemInOrder(int bookId, int count = 1)
        {
            (Cart cart, Order order) = CreateOrGetCartAndOrder();

            var book = _bookRepository.GetById(bookId);
            if (order.Items.TryGet(bookId, out OrderItem? orderItem))
                orderItem!.Count += count;
            else
                order.Items.Add(bookId, book.Price, count);

            SaveOrderAndCart(order, cart);

            return RedirectToAction("Index", "Book", new { BookId = bookId });
        }

        [HttpPost]
        public IActionResult RemoveOrderItem(int bookId)
        {
            (Cart cart, Order order) = CreateOrGetCartAndOrder();

            order.Items.Remove(bookId);
            SaveOrderAndCart(order, cart);

            return RedirectToAction("Index");
        }
        public IActionResult UpdateOrderItem(int bookId, int count = 1)
        {
            (Cart cart, Order order) = CreateOrGetCartAndOrder();
            var book = _bookRepository.GetById(bookId);

            order.Items.Get(bookId).Count += count; // todo:
            SaveOrderAndCart(order, cart);

            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult SendConfirmationCode(int orderId, string cellPhone)
        {
            var order = _orderRepository.GetById(orderId);
            var orderModel = Map(order);

            if(!IsValidCellPhone(cellPhone))
            {
                orderModel.Errors[nameof(cellPhone)] = "Номер телефона не соотвецтвует формату +79876543210.";
                return View("Index", orderModel);
            }
            int code = 1111;
            HttpContext.Session.SetInt32(cellPhone, code);

            //заглушка реализации
            _notificationService.SendConfirmationCode(cellPhone, code);

            return View("Confirmation", new ConfirmationModel 
            {OrderId = order.OrderId, CellPhone = cellPhone });
        }
        [HttpPost]
        public IActionResult Confirmate(int orderId, string cellPhone, int code)
        {
            int? storedCode = HttpContext.Session.GetInt32(cellPhone);
            if (storedCode == null)
            {
                return View("Confirmation",
                            new ConfirmationModel
                            {
                                OrderId = orderId,
                                CellPhone = cellPhone,
                                Errors = new Dictionary<string, string>
                                {
                                    { "code", "Пустой код, повторите отправку" }
                                },
                            }); ;
            }

            if (storedCode != code)
            {
                return View("Confirmation",
                            new ConfirmationModel
                            {
                                OrderId = orderId,
                                CellPhone = cellPhone,
                                Errors = new Dictionary<string, string>
                                {
                                    { "code", "Отличается от отправленного" }
                                },
                            }); ;
            }

            var order = _orderRepository.GetById(orderId);
            order.CellPhone = cellPhone;
            _orderRepository.Update(order);

            HttpContext.Session.Remove(cellPhone);

            var deliveryModel = new DeliveryModel
            {
                OrderId = orderId,
                Methods = _deliveryServices.ToDictionary(service => service.UniqueCode,
                                                         service => service.Title)
            };

            return View("DeliveryMethod", deliveryModel);
        }
        [HttpPost]
        public IActionResult StartDelivery(int orderId, string uniqueCode)
        {
            var deliveryService = _deliveryServices.Single(service => service.UniqueCode == uniqueCode);
            var order = _orderRepository.GetById(orderId);

            var form = deliveryService.CreateForm(order);

            return View("DeliveryStep", form);
        }

        [HttpPost]
        public IActionResult NextDelivery(int orderId, string uniqueCode, int step, Dictionary<string, string> values)
        {
            var deliveryService = _deliveryServices.Single(service => service.UniqueCode == uniqueCode);

            var form = deliveryService.MoveNextForm(orderId, step, values); 

            if (form.IsFinal)
            {
                var order = _orderRepository.GetById(orderId);
                order.Delivery = deliveryService.GetOrderDelivery(form);
                _orderRepository.Update(order);

                var testcodes = _paymentServices.Select(service => service.UniqueCode);
                var testTitles = _paymentServices.Select(service => service.Title);
                var deliveryModel = new DeliveryModel
                {
                    OrderId = orderId,
                    Methods = _paymentServices.ToDictionary(service => service.UniqueCode,
                                                            service => service.Title)
                };

                return View("PaymentMethod", deliveryModel);
            }

            return View("DeliveryStep", form);
        }
        public IActionResult StartPayment(int orderId, string uniqueCode)
        {
            var paymentService = _paymentServices.Single(service => service.UniqueCode == uniqueCode);
            var order = _orderRepository.GetById(orderId);

            var form = paymentService.CreateForm(order);

            var webContractorservice = _webContractorServices
                                       .SingleOrDefault(service => service.UniqueCode == uniqueCode);
            if (webContractorservice != null)
                return Redirect(webContractorservice.GetUri);

            return View("PaymentStep", form);
        }

        [HttpPost]
        public IActionResult NextPayment(int orderId, string uniqueCode, int step, Dictionary<string, string> values)
        {
            var paymentService = _paymentServices.Single(service => service.UniqueCode == uniqueCode);

            var form = paymentService.MoveNextForm(orderId, step, values);

            if (form.IsFinal)
            {
                var order = _orderRepository.GetById(orderId);
                order.Payment = paymentService.GetOrderPayment(form);
                _orderRepository.Update(order);
               
                return RedirectToAction(nameof(OrderController.Finish));
            }

            return View("PaymentStep", form);
        }
        public IActionResult Finish()
        {
            HttpContext.Session.RemoveCart();
            return View();
        }

        private OrderModel Map(Order order)
        {
            var bookIds = order.Items.Select(book => book.BookId);
            var books = _bookRepository.GetAllByIds(bookIds);
            var orderItemsModel = from orderItem in order.Items
                             join book in books
                             on orderItem.BookId equals book.BookId
                             select new OrderItemModel
                             {
                                 BookId = book.BookId,
                                 Title = book.Title,
                                 Author = book.Author,
                                 Price = book.Price,
                                 Count = orderItem.Count
                             };

            return new OrderModel
            {
                OrderId = order.OrderId,
                OrderItems = orderItemsModel.ToArray(),
                TotalCount = order.TotalCount,
                TotalPrice = order.TotalPrice,
                DeliveryPrice = order.Delivery?.DeliveryPrice                
            };
        }
        private (Cart cart, Order order) CreateOrGetCartAndOrder()
        {
            Order order;
            if (HttpContext.Session.TryGetCart(out Cart? cart))
            {
                order = _orderRepository.GetById(cart!.OrderId);
            }
            else
            {
                order = _orderRepository.Create();
                cart = new Cart(order.OrderId);
            }
            return (cart, order);
        }
        private void SaveOrderAndCart(Order order, Cart cart)
        {
            _orderRepository.Update(order);

            cart.TotalCount = order.TotalCount;
            cart.TotalPrice = order.TotalPrice;

            HttpContext.Session.Set(cart);
        }
        private bool IsValidCellPhone(string cellPhone)
        {
            if (cellPhone == null)
                return false;

            cellPhone = cellPhone.Replace(" ", "")
                                 .Replace("-", "");

            return Regex.IsMatch(cellPhone, @"^\+?\d{11}$");
        }

    }
}

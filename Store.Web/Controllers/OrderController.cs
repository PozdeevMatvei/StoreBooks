using Microsoft.AspNetCore.Mvc;
using Store.Web.Models;
using Store.Messages;
using System.Text.RegularExpressions;

namespace Store.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IBookRepository _bookRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly INotificationService _notificationService;

        public OrderController(IBookRepository bookRepository, IOrderRepository orderRepository,
            INotificationService notificationService)
        {
            _bookRepository = bookRepository;
            _orderRepository = orderRepository;
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
        public IActionResult AddItemInOrder(int bookId)
        {
            var (cart, order) = CreateOrGetCartAndOrder();

            var book = _bookRepository.GetById(bookId);
            order.AddOrUpdateOrderItem(book, 1);

            SaveOrderAndCart(order, cart);

            return RedirectToAction("Index", "Book", new { BookId = bookId });
        }
        [HttpPost]
        public IActionResult RemoveOrderItem(int bookId)
        {
            (Cart cart, Order order) = CreateOrGetCartAndOrder();

            order.RemoveOrderItem(bookId);
            SaveOrderAndCart(order, cart);

            return RedirectToAction("Index");
        }
        public IActionResult UpdateOrderItem(int bookId, int count = 1)
        {
            (Cart cart, Order order) = CreateOrGetCartAndOrder();
            var book = _bookRepository.GetById(bookId);

            order.AddOrUpdateOrderItem(book, count);
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
            _notificationService.SendConfirmationCode(cellPhone, code);

            return View("Confirmation", new ConfirmationModel 
            {OrderId = order.OrderId, CellPhone = cellPhone });
        }
        [HttpPost]
        public IActionResult StartDelivery(int orderiId, string cellPhone, int code)
        {
            int? storedCode = HttpContext.Session.GetInt32(cellPhone);
            if (storedCode == null)
            {
                return View("Confirmation",
                            new ConfirmationModel
                            {
                                OrderId = orderiId,
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
                                OrderId = orderiId,
                                CellPhone = cellPhone,
                                Errors = new Dictionary<string, string>
                                {
                                    { "code", "Отличается от отправленного" }
                                },
                            }); ;
            }

            return View();
        }

        private OrderModel Map(Order order)
        {
            var bookIds = order.Items.Select(book => book.BookId);
            var books = _bookRepository.GetAllByIds(bookIds);
            var itemModels = from orderItem in order.Items
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
                OrderItems = itemModels.ToArray(),
                TotalCount = order.TotalCount,
                TotalPrice = order.TotalPrice
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

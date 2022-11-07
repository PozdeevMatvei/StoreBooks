using Microsoft.AspNetCore.Mvc;
using Store.Web.Models;

namespace Store.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IBookRepository _bookRepository;
        private readonly IOrderRepository _orderRepository;

        public OrderController(IBookRepository bookRepository, IOrderRepository orderRepository)
        {
            _bookRepository = bookRepository;
            _orderRepository = orderRepository;
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
        public IActionResult AddItemInOrder(int bookId)
        {
            Order order;
            //HttpContext.Session.GetCart(out cart);

            if (HttpContext.Session.TryGetCart(out Cart? cart))
            {
                order = _orderRepository.GetById(cart!.OrderId);
            }
            else
            {
                order = _orderRepository.Create();
                cart = new Cart(order.OrderId);
            }

            var book = _bookRepository.GetById(bookId);
            order.AddItem(book, 1);
            _orderRepository.Update(order);

            cart.TotalCount = order.TotalCount;
            cart.TotalPrice = order.TotalPrice;

            HttpContext.Session.Set(cart);

            return RedirectToAction("Index", "Book", new { BookId = bookId});
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
    }
}

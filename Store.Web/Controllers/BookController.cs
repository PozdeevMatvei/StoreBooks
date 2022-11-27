using Microsoft.AspNetCore.Mvc;
using Store.Web.App;

namespace Store.Web.Controllers
{
    public class BookController : Controller
    {
        private readonly BookService _bookService;
        private readonly OrderService _orderService;

        public BookController(BookService bookService, OrderService orderService)
        {
            _bookService = bookService;
            _orderService = orderService;
        }

        public IActionResult Index(int bookId)
        {
            BookModel book = _bookService.GetById(bookId);
            book.IsBookCart = _orderService.IsBookInCart(bookId);

            return View(book);
        }
    }
}

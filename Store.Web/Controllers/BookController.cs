using Microsoft.AspNetCore.Mvc;
using Store.Web.App.Models;
using Store.Web.App.Services;

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

        public async Task<IActionResult> Index(int bookId)
        {
            BookModel book = await _bookService.GetByIdAsync(bookId);
            book.IsBookCart = await _orderService.IsBookInCartAsync(bookId);

            return View(book);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Store.Web.App.Services;

namespace Store.Web.Controllers
{
    public class SearchController : Controller
    {
        private readonly BookService _bookService;
        public SearchController(BookService bookService)
        {
            _bookService = bookService;
        }
        public async Task<IActionResult> Index(string query)
        {
            var books = await _bookService.GetAllByQueryAsync(query);
            return View(books);
        }
    }
}

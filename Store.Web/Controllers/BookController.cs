using Microsoft.AspNetCore.Mvc;

namespace Store.Web.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookRepository _bookRepository;

        public BookController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public IActionResult Index(int bookId)
        {
            Book book = _bookRepository.GetById(bookId);

            return View(book);
        }
    }
}

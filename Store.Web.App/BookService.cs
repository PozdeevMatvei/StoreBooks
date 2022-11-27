using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Web.App
{
    public class BookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public BookModel GetById(int bookId)
        {
            Book book = _bookRepository.GetById(bookId);

            return Map(book);
        }
        public IReadOnlyCollection<BookModel> GetAllByQuery(string query)
        {
            var books = Book.IsIsbn(query)
                        ? _bookRepository.GetAllByIsbn(query)
                        : _bookRepository.GetAllByTitleOrAuthor(query);

            return books.Select(Map).ToArray();
        }

        private BookModel Map(Book book)
        {
            return new BookModel
            {
                BookId = book.BookId,
                Isbn = book.Isbn,
                Title = book.Title,
                Author = book.Author,
                Description = book.Description,
                Price = book.Price
            };
        }
    }
}

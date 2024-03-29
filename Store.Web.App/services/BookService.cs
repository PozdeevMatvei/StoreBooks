﻿using Store.Web.App.Models;

namespace Store.Web.App.Services
{
    public class BookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<BookModel> GetByIdAsync(int bookId)
        {
            Book book = await _bookRepository.GetByIdAsync(bookId);

            return Map(book);
        }
        public async Task<IReadOnlyCollection<BookModel>> GetAllByQueryAsync(string query)
        {
            if (query == null)
                return Array.Empty<BookModel>();

            var books = Book.IsIsbn(query)
                        ? await _bookRepository.GetAllByIsbnAsync(query)
                        : await _bookRepository.GetAllByTitleOrAuthorAsync(query);

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

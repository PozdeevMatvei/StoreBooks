using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.DTO.EF
{
    public class BookRepository : IBookRepository
    {
        private readonly StoreDbContextFactory _dbContextFactory; 

        public BookRepository(StoreDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public Book[] GetAllByIds(IEnumerable<int> bookIds)
        {
            var dbContext = _dbContextFactory.GetOrCreate(typeof(BookRepository));
            var bookDtos = dbContext.Books
                                         .Where(book => bookIds.Contains(book.BookId))
                                         .ToArray();

            return bookDtos.Select(Book.Mapper.Map)
                           .ToArray();
        }

        public Book[] GetAllByIsbn(string isbn)
        {
            if(Book.TryFormatIsbn(isbn, out string? formatedIsbn))
            {
                var dbContext = _dbContextFactory.GetOrCreate(typeof(BookRepository));
                var bookDtos = dbContext.Books
                                             .Where(book => book.Isbn == formatedIsbn)
                                             .ToArray();

                return bookDtos.Select(Book.Mapper.Map)
                               .ToArray();
            }

            return Array.Empty<Book>();
        }

        public Book[] GetAllByTitleOrAuthor(string titleOrAuthor)
        {
            var dbContext = _dbContextFactory.GetOrCreate(typeof(BookRepository));
            var bookDtos = dbContext.Books
                                    .Where(book => book.Title
                                                       .ToLower()
                                                       .Contains(titleOrAuthor.ToLower())
                                                || book.Author != null 
                                                && book.Author
                                                       .ToLower()
                                                       .Contains(titleOrAuthor.ToLower()))
                                    .ToArray();
            

            return bookDtos.Select(Book.Mapper.Map).ToArray();            
        }

        public Book GetById(int id)
        {
            var dbContext = _dbContextFactory.GetOrCreate(typeof(BookRepository));
            var bookDto = dbContext.Books.Single(book => book.BookId == id);

            return Book.Mapper.Map(bookDto);
        }
    }
}

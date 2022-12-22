using Microsoft.EntityFrameworkCore;

namespace Store.DTO.EF
{
    public class BookRepository : IBookRepository
    {
        private readonly StoreDbContextFactory _dbContextFactory;

        public BookRepository(StoreDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<Book[]> GetAllByIdsAsync(IEnumerable<int> bookIds)
        {
            var dbContext = _dbContextFactory.GetOrCreate(typeof(BookRepository));
            var bookDtos = await dbContext.Books
                                         .Where(book => bookIds.Contains(book.Id))
                                         .ToArrayAsync();

            return bookDtos.Select(Book.Mapper.Map)
                           .ToArray();
        }

        public async Task<Book[]> GetAllByIsbnAsync(string isbn)
        {
            if (Book.TryFormatIsbn(isbn, out string? formatedIsbn))
            {
                var dbContext = _dbContextFactory.GetOrCreate(typeof(BookRepository));
                var bookDtos = await dbContext.Books
                                             .Where(book => book.Isbn == formatedIsbn)
                                             .ToArrayAsync();

                return bookDtos.Select(Book.Mapper.Map)
                               .ToArray();
            }

            return Array.Empty<Book>();
        }

        public async Task<Book[]> GetAllByTitleOrAuthorAsync(string titleOrAuthor)
        {
            var dbContext = _dbContextFactory.GetOrCreate(typeof(BookRepository));
            var bookDtos = await dbContext.Books
                                    .Where(book => book.Title
                                                       .ToLower()
                                                       .Contains(titleOrAuthor.ToLower())
                                                || book.Author != null
                                                && book.Author
                                                       .ToLower()
                                                       .Contains(titleOrAuthor.ToLower()))
                                    .ToArrayAsync();


            return bookDtos.Select(Book.Mapper.Map).ToArray();
        }

        public async Task<Book> GetByIdAsync(int id)
        {
            var dbContext = _dbContextFactory.GetOrCreate(typeof(BookRepository));            
            var bookDto = await dbContext.Books.SingleAsync(book => book.Id == id);
            
            return Book.Mapper.Map(bookDto);
        }
    }
}

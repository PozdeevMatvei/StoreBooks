namespace Store.Memory
{
    public record class MockBook(int id, string isbn, string title, 
                                  string author, string description, decimal price)
    {
        public Book Map()
        {
            var bookDto =  Book.DtoFactory.Create(this.isbn,
                                                  this.author,
                                                  this.description,
                                                  this.title,
                                                  this.price);
            bookDto.BookId = this.id;
            return new Book(bookDto);
        }
    }

    public class MockBookRepository : IBookRepository
    {
        private List<Book> _books = new List<Book>();
        public MockBookRepository()
        {
            BooksInitial();
        }       

        public Book[] Books => _books.ToArray();
        public Book[] GetAllByIsbn(string isbn)
        {
            return _books.Where(book => book.Isbn == isbn).ToArray();
        }

        public Book[] GetAllByTitleOrAuthor(string query)
        {
            if(string.IsNullOrEmpty(query))
                throw new ArgumentNullException(nameof(query));



            return _books.Where(book => book.Title.ToLower().Contains(query.ToLower()) 
                                     || 
                                     book.Author != null && 
                                     book.Author.ToLower().Contains(query.ToLower()))
                         .ToArray();
        }

        public Book[] GetAllByIds(IEnumerable<int> bookIds)
        {
            var foundBooks = from book in _books
                             join bookId in bookIds
                             on book.BookId equals bookId
                             select book;

            return foundBooks.ToArray();
        }

        public Book GetById(int bookId)
        {
            return _books.Single(book => book.BookId == bookId);
        }

        private void BooksInitial()
        {
            _books.Add(new MockBook(1, "ISBN 11111-12341", "Clr via c#", "Джефри Рихтер", "description", 10m).Map());
            _books.Add(new MockBook(2, "ISBN 11111-12342", "C# 4.0", "Герберт Шилдт", "description", 8m).Map());
            _books.Add(new MockBook(3, "ISBN 11111-12343", "Clear Code", "Роберт Мартин", "description", 12m).Map());
        }
    }
}
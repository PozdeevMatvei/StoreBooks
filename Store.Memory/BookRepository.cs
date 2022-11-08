namespace Store.Memory
{
    public class BookRepository : IBookRepository
    {
        private readonly Book[] _books = new[]
        {
            new Book(1, "ISBN 11111-12341", "Clr via c#", "Джефри Рихтер", "description", 10m ),
            new Book(2, "ISBN 11111-12342", "C# 4.0", "Герберт Шилдт", "description", 8m),
            new Book(3, "ISBN 11111-12343", "Clear Code", "Роберт Мартин", "description", 12m)
        };
       

        public Book[] GetAllByIsbn(string isbn)
        {
            return _books.Where(book => book.Isbn == isbn).ToArray();
        }

        public Book[] GetAllByTitleOrAuthor(string query)
        {
            if(string.IsNullOrEmpty(query))
                throw new ArgumentNullException(nameof(query));



            return _books.Where(book => book.Title.ToLower().Contains(query.ToLower()) 
                                     || book.Author.ToLower().Contains(query.ToLower()))
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
    }
}
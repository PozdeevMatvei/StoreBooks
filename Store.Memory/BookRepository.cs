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
            return _books.Where(book => book.Title.Contains(query)
                                     || book.Author.Contains(query))
                         .ToArray();
        }
    }
}
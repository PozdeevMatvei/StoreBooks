namespace Store.Memory
{
    public class BookRepository : IBookRepository
    {
        private readonly Book[] _books = new[]
        {
            new Book(1, "ISBN 11111-12341", "Clr via c#", "Джефри Рихтер"),
            new Book(2, "ISBN 11111-12342", "C# 4.0", "Герберт Шилдт"),
            new Book(3, "ISBN 11111-12343", "Clear Code", "Роберт Мартин")
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
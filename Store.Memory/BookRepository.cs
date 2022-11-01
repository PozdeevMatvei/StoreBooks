namespace Store.Memory
{
    public class BookRepository : IBookRepository
    {
        private readonly Book[] _books = new[]
        {
            new Book(1, "Clr via c#"),
            new Book(2, "C# 4.0"),
            new Book(3, "Clear Code")
        };
        public Book[] GetAllByTitle(string title)
        {
            return _books.Where(book => book.Title.Contains(title)).ToArray();
        }
    }
}
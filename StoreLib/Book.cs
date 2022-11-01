namespace Store
{
    public class Book
    {
        public int BookId { get; }
        public string Title { get; }

        public Book(int bookId, string title)
        {
            BookId = bookId;
            Title = title;
        }
    }
}
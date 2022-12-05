namespace Store.Web.App
{
    public class BookModel
    {
        public int BookId { get; set; }
        public string Isbn { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Author { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public bool IsBookCart { get; set; }
    }
}
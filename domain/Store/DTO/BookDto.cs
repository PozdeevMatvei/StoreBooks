namespace Store.DTO
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Isbn { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Author { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
    }
}

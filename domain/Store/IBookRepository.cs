namespace Store
{
    public interface IBookRepository
    {
        Task<Book[]> GetAllByIsbnAsync(string isbn);
        Task<Book[]> GetAllByTitleOrAuthorAsync(string titleOrAuthor);
        Task<Book[]> GetAllByIdsAsync(IEnumerable<int> bookIds);
        Task<Book> GetByIdAsync(int id);
    }
}

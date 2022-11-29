using Store.DTO;
using System.Text.RegularExpressions;

namespace Store
{
    public class Book
    {
        private BookDto _dto;
        public int BookId => _dto.BookId;
        public string Isbn
        {
            get => _dto.Isbn;
            set
            {
                if (TryFormatIsbn(value, out string? formatedIsbn))
                    _dto.Isbn = formatedIsbn!;

                throw new ArgumentException(nameof(Isbn));
            }
        }
        public string Title
        {
            get => _dto.Title;
            set
            {
                if(string.IsNullOrEmpty(value))
                    throw new ArgumentException(nameof(Title));

                _dto.Title = value.Trim();
            }
        }
        public string? Author
        {
            get => _dto.Author;
            set => _dto.Author = value?.Trim();
        }
        public string? Description
        {
            get => _dto.Description;
            set => _dto.Description = value?.Trim();
        }
        public decimal Price
        {
            get => _dto.Price;
            set
            {
                if(value > 0)
                    _dto.Price = value;

                throw new ArgumentException(nameof(Price));
            }
        }

        public Book(BookDto dto)
        {
            _dto = dto;
        }

        public static class DtoFactory
        {
            public static BookDto Create(string isbn,
                                         string title,
                                         string author,
                                         string description,
                                         decimal price)
            {
                if(TryFormatIsbn(isbn, out string? formatedIsbn))
                    isbn = formatedIsbn!;
                else
                    throw new ArgumentException(nameof(isbn));

                if(string.IsNullOrEmpty(title))
                    throw new ArgumentException(nameof(title));

                if (price <= 0)
                    throw new ArgumentException(nameof(price));

                return new BookDto
                {
                    Isbn = isbn,
                    Title = title.Trim(),
                    Author = author?.Trim(),
                    Description = description?.Trim(),
                    Price = price
                };
            }
        }
        public static class Mapper
        {
            public static Book Map(BookDto dto) => new Book(dto);
            public static BookDto Map(Book book) => book._dto;
        }
        public static bool TryFormatIsbn(string? isbn, out string? formatedIsbn)
        {
            if (isbn == null)
            {
                formatedIsbn = null;
                return false;
            }

            formatedIsbn = isbn.Replace("-", "")
                               .Replace(" ", "")
                               .ToUpper();

            return Regex.IsMatch(formatedIsbn, @"^ISBN\d{10}(\d{3})?$");

        }
        public static bool IsIsbn(string? query) => TryFormatIsbn(query, out _);
        
    }
}
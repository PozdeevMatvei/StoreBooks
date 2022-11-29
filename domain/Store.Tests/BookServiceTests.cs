using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Store.Web.App;
using Store.DTO;

namespace Store.Tests
{
    public class BookServiceTests
    {
        private Book CreateTestBook(string isbn = "ISBN 11111-12341",
                            string title = "Clr via c#",
                            string author = "Джефри Рихтер",
                            string description = "description",
                            decimal price = 10m)
        {
            var bookDto = Book.DtoFactory.Create(isbn, title, author, description, price);
            return new Book(bookDto);
        }
        [Fact]
        public void GetAllByQuery_WithIsbn_GetAllByIsbn()
        {
            var bookRepositoryStub = new Mock<IBookRepository>();

            bookRepositoryStub.Setup(bookRepository => bookRepository
                .GetAllByIsbn(It.IsAny<string>()))
                .Returns(new[] { CreateTestBook() });


            bookRepositoryStub.Setup(bookRepository => bookRepository
                .GetAllByTitleOrAuthor(It.IsAny<string>()))
                .Returns(new[] { CreateTestBook() });

            var bookService = new BookService(bookRepositoryStub.Object);

            var validIsbn = "Isbn 12345-12345";
            var actual = bookService.GetAllByQuery(validIsbn);

            Assert.Collection(actual, book => Assert.Equal(1, book.BookId));
        }

        [Fact]
        public void GetAllByQuery_WithIsbn_GetAllByTitleOrAuthor()
        {
            var bookRepositoryStub = new Mock<IBookRepository>();

            bookRepositoryStub.Setup(bookRepository => bookRepository
                .GetAllByIsbn(It.IsAny<string>()))
                .Returns(new[] { CreateTestBook() });


            bookRepositoryStub.Setup(bookRepository => bookRepository
                .GetAllByTitleOrAuthor(It.IsAny<string>()))
                .Returns(new[] { CreateTestBook() });

            var bookService = new BookService(bookRepositoryStub.Object);

            var invalidIsbn = "A.S. Pushkin";
            var actual = bookService.GetAllByQuery(invalidIsbn);

            Assert.Collection(actual, book => Assert.Equal(2, book.BookId));
        }
    }
}

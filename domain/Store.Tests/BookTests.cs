namespace Store.Tests
{
    public class BookTests
    {
        [Fact]
        public void IsIsbn_WithNull_False()
        {
            bool actual = Book.IsIsbn(null);

            Assert.False(actual);
        }
        [Fact]
        public void IsIsbn_WithBlancString_False()
        {
            bool actual = Book.IsIsbn("    ");

            Assert.False(actual);
        }
        [Fact]
        public void IsIsbn_WithInvalidIsbn_False()
        {
            bool actual = Book.IsIsbn("ISBN 12345-3");

            Assert.False(actual);
        }
        [Fact]
        public void IsIsbn_WithTrashIsbn_False()
        {
            bool actual = Book.IsIsbn("fff IsbN 102-303-978 0 123 assd");
            Assert.False(actual);
        }


        [Fact]
        public void IsIsbn_WithValidIsbn10Digits_True()
        {
            bool actual = Book.IsIsbn("IsbN 102-303-978 0");
            Assert.True(actual);
        }
        [Fact]
        public void IsIsbn_WithValidIsbn13Digits_True()
        {
            bool actual = Book.IsIsbn("IsbN 102-303-978 0 123");
            Assert.True(actual);
        }
        
    }
}
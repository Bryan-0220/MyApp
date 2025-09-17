using Domain.Common;
using Domain.Models;

namespace Tests.Domain
{
    public class BookTests
    {
        [Fact]
        public void Create_ValidData_ReturnsBook()
        {
            var data = new BookData
            {
                Title = "The Hobbit",
                AuthorId = "author-1",
                ISBN = "97802",
                PublishedYear = 1937,
                CopiesAvailable = 3,
                Genre = "Fantasy"
            };

            var book = Book.Create(data);

            Assert.NotNull(book);
            Assert.Equal("The Hobbit", book.Title);
            Assert.Equal("author-1", book.AuthorId);
            Assert.Equal(3, book.CopiesAvailable);
            Assert.Equal("Fantasy", book.Genre);
            Assert.Equal("97802", book.ISBN);
            Assert.Equal(1937, book.PublishedYear);
        }

        [Fact]
        public void Create_NullInput_ThrowsDomainException()
        {
            Assert.Throws<DomainException>(() => Book.Create(null!));
        }

        [Fact]
        public void Create_InvalidIsbn_ThrowsDomainException()
        {
            var data = new BookData
            {
                Title = "Invalid ISBN Book",
                AuthorId = "a1",
                ISBN = "not-an-isbn",
                CopiesAvailable = 1,
                Genre = "Fiction"
            };

            Assert.Throws<DomainException>(() => Book.Create(data));
        }

        [Theory]
        [InlineData(null, "a1", "97801", 1, "Fiction")]
        [InlineData("   ", "a1", "97801", 1, "Fiction")]
        [InlineData("Title", null, "97801", 1, "Fiction")]
        [InlineData("Title", "   ", "97801", 1, "Fiction")]
        [InlineData("Title", "a1", "not-an-isbn", 1, "Fiction")]
        [InlineData("Title", "a1", "97801", -1, "Fiction")]
        [InlineData("Title", "a1", "97801", 1, null)]
        [InlineData("Title", "a1", "97801", 1, "   ")]
        public void Create_MissingOrInvalidFields_Throws(string? title, string? authorId, string? isbn, int copies, string? genre)
        {
            var data = new BookData
            {
                Title = title!,
                AuthorId = authorId!,
                ISBN = isbn,
                CopiesAvailable = copies,
                Genre = genre!
            };

            Assert.Throws<DomainException>(() => Book.Create(data));
        }

        [Fact]
        public void EnsureHasAvailableCopies_WhenZero_ThrowsDomainException()
        {
            var data = new BookData
            {
                Title = "Zero Copies",
                AuthorId = "a1",
                CopiesAvailable = 0,
                Genre = "Sci-Fi"
            };

            var book = Book.Create(data);

            Assert.Throws<DomainException>(() => book.EnsureHasAvailableCopies());
        }


        [Fact]
        public void SetCopiesAvailable_Negative_Throws()
        {
            var data = new BookData { Title = "T", AuthorId = "A", CopiesAvailable = 1, Genre = "G" };
            var b = Book.Create(data);
            Assert.Throws<DomainException>(() => b.SetCopiesAvailable(-5));
        }

        [Fact]
        public void SetGenre_NormalizesString()
        {
            var data = new BookData { Title = "T", AuthorId = "A", CopiesAvailable = 1, Genre = "G" };
            var b = Book.Create(data);
            b.SetGenre("  Science   Fiction  ");
            Assert.Contains("Science Fiction", b.Genre);
        }

    }

}

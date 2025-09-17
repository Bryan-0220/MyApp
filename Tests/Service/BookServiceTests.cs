using Application.Books.Services;
using Application.Interfaces;
using Domain.Models;
using Domain.Common;
using FakeItEasy;

namespace Tests
{
    public class BookServiceTests
    {
        [Fact]
        public async Task GivenBookDoesNotExist_WhenDeleteBook_ThenReturnsFail()
        {
            // Arrange
            var repo = A.Fake<IBookRepository>();
            var loanRepo = A.Fake<ILoanRepository>();

            A.CallTo(() => repo.GetById(A<string>._, A<CancellationToken>._)).Returns(Task.FromResult<Book?>(null));

            var service = new BookService(repo, loanRepo);

            // Act
            var result = await service.DeleteBook("non-existent-id");

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Book not found.", result.Message);
        }

        [Fact]
        public async Task GivenBookHasActiveLoans_WhenDeleteBook_ThenReturnsFail()
        {
            // Arrange
            var repo = A.Fake<IBookRepository>();
            var loanRepo = A.Fake<ILoanRepository>();

            var bookData = new BookData { Title = "T", AuthorId = "a", CopiesAvailable = 1, Genre = "g" };
            var book = Book.Create(bookData);
            book.Id = "book-1";

            A.CallTo(() => repo.GetById("book-1", A<CancellationToken>._)).Returns(Task.FromResult<Book?>(book));
            A.CallTo(() => loanRepo.Filter(A<Application.Filters.LoanFilter>._, A<CancellationToken>._))
                .Returns(new List<Loan> { new Loan { Id = "loan-1", BookId = "book-1", ReaderId = "r1" } });

            var service = new BookService(repo, loanRepo);

            // Act
            var result = await service.DeleteBook("book-1");

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Book cannot be deleted while it has active loans.", result.Message);
        }

        [Fact]
        public async Task GivenBookExistsAndNoActiveLoans_WhenDeleteBook_ThenDeletesAndReturnsOk()
        {
            // Arrange
            var repo = A.Fake<IBookRepository>();
            var loanRepo = A.Fake<ILoanRepository>();

            var bookData = new BookData { Title = "T", AuthorId = "a", CopiesAvailable = 1, Genre = "g" };
            var book = Book.Create(bookData);
            book.Id = "book-1";

            A.CallTo(() => repo.GetById("book-1", A<CancellationToken>._)).Returns(Task.FromResult<Book?>(book));
            A.CallTo(() => loanRepo.Filter(A<Application.Filters.LoanFilter>._, A<CancellationToken>._)).Returns(new List<Loan>());
            A.CallTo(() => repo.Delete("book-1", A<CancellationToken>._)).Returns(Task.FromResult(true));

            var service = new BookService(repo, loanRepo);

            // Act
            var result = await service.DeleteBook("book-1");

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Book deleted.", result.Message);
            Assert.Equal("book-1", result.Value?.Id);

            A.CallTo(() => repo.Delete("book-1", A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        }
    }
}

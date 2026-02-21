using Application.Books.Services;
using UpdateBook;
using Application.Filters;
using Application.Interfaces;
using Domain.Models;
using FakeItEasy;
using Domain.Common;

namespace Tests
{
    public class BookServiceTests
    {
        [Fact]
        public async Task GetBookOrThrow_ShouldReturnBook_WhenExists()
        {
            var repo = A.Fake<IBookRepository>();
            var loanRepo = A.Fake<ILoanRepository>();
            var book = Book.Create(new BookData { Title = "T", AuthorId = "a", CopiesAvailable = 1, Genre = "g" });
            book.Id = "b1";
            A.CallTo(() => repo.GetById("b1", A<CancellationToken>._)).Returns(Task.FromResult<Book?>(book));
            var service = new BookService(repo, loanRepo);
            var result = await service.GetBookOrThrow("b1");
            Assert.Equal("b1", result.Id);
        }

        [Fact]
        public async Task GetBookOrThrow_ShouldThrowNotFound_WhenMissing()
        {
            var repo = A.Fake<IBookRepository>();
            var loanRepo = A.Fake<ILoanRepository>();
            A.CallTo(() => repo.GetById("b2", A<CancellationToken>._)).Returns(Task.FromResult<Book?>(null));
            var service = new BookService(repo, loanRepo);
            await Assert.ThrowsAsync<NotFoundException>(() => service.GetBookOrThrow("b2"));
        }

        [Fact]
        public async Task DecreaseCopiesOrThrow_ShouldSucceed_WhenCopiesAvailable()
        {
            var repo = A.Fake<IBookRepository>();
            var loanRepo = A.Fake<ILoanRepository>();
            A.CallTo(() => repo.TryChangeCopies("b1", -1, A<CancellationToken>._)).Returns(Task.FromResult(true));
            var service = new BookService(repo, loanRepo);
            await service.DecreaseCopiesOrThrow("b1");
            A.CallTo(() => repo.TryChangeCopies("b1", -1, A<CancellationToken>._)).MustHaveHappened();
        }

        [Fact]
        public async Task DecreaseCopiesOrThrow_ShouldThrowBusinessRule_WhenNoCopies()
        {
            var repo = A.Fake<IBookRepository>();
            var loanRepo = A.Fake<ILoanRepository>();
            A.CallTo(() => repo.TryChangeCopies("b1", -1, A<CancellationToken>._)).Returns(Task.FromResult(false));
            var service = new BookService(repo, loanRepo);
            await Assert.ThrowsAsync<BusinessRuleException>(() => service.DecreaseCopiesOrThrow("b1"));
        }

        [Fact]
        public async Task RestoreCopies_ShouldSucceed_WhenPossible()
        {
            var repo = A.Fake<IBookRepository>();
            var loanRepo = A.Fake<ILoanRepository>();
            A.CallTo(() => repo.TryChangeCopies("b1", 1, A<CancellationToken>._)).Returns(Task.FromResult(true));
            var service = new BookService(repo, loanRepo);
            await service.RestoreCopies("b1");
            A.CallTo(() => repo.TryChangeCopies("b1", 1, A<CancellationToken>._)).MustHaveHappened();
        }

        [Fact]
        public async Task RestoreCopies_ShouldThrowBusinessRule_WhenFails()
        {
            var repo = A.Fake<IBookRepository>();
            var loanRepo = A.Fake<ILoanRepository>();
            A.CallTo(() => repo.TryChangeCopies("b1", 1, A<CancellationToken>._)).Returns(Task.FromResult(false));
            var service = new BookService(repo, loanRepo);
            await Assert.ThrowsAsync<BusinessRuleException>(() => service.RestoreCopies("b1"));
        }

        [Fact]
        public async Task UpdateBook_ShouldUpdateAndReturn_WhenExists()
        {
            var repo = A.Fake<IBookRepository>();
            var loanRepo = A.Fake<ILoanRepository>();
            var book = Book.Create(new BookData { Title = "T", AuthorId = "a", CopiesAvailable = 1, Genre = "g" });
            book.Id = "b1";
            A.CallTo(() => repo.GetById("b1", A<CancellationToken>._)).Returns(Task.FromResult<Book?>(book));
            A.CallTo(() => repo.Update(book, A<CancellationToken>._)).Returns(Task.FromResult(true));
            var service = new BookService(repo, loanRepo);
            var input = new UpdateBookCommandInput { Id = "b1", Title = "New Title" };
            var updated = await service.UpdateBook(input);
            Assert.Equal("New Title", updated.Title);
        }

        [Fact]
        public async Task UpdateBook_ShouldThrowNotFound_WhenMissing()
        {
            var repo = A.Fake<IBookRepository>();
            var loanRepo = A.Fake<ILoanRepository>();
            A.CallTo(() => repo.GetById("b2", A<CancellationToken>._)).Returns(Task.FromResult<Book?>(null));
            var service = new BookService(repo, loanRepo);
            var input = new UpdateBookCommandInput { Id = "b2", Title = "T" };
            await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateBook(input));
        }

        [Fact]
        public async Task UpdateBook_ShouldThrowDuplicate_WhenIsbnUsedByAnother()
        {
            var repo = A.Fake<IBookRepository>();
            var loanRepo = A.Fake<ILoanRepository>();
            var book = Book.Create(new BookData { Title = "T", AuthorId = "a", CopiesAvailable = 1, Genre = "g", ISBN = "12345" });
            book.Id = "b1";

            var another = Book.Create(new BookData { Title = "X", AuthorId = "a", CopiesAvailable = 1, Genre = "g", ISBN = "54321" });
            another.Id = "b2";

            A.CallTo(() => repo.GetById("b1", A<CancellationToken>._)).Returns(Task.FromResult<Book?>(book));
            A.CallTo(() => repo.Count(A<System.Linq.Expressions.Expression<Func<Book, bool>>>._, A<CancellationToken>._)).Returns(1);

            var service = new BookService(repo, loanRepo);
            var input = new UpdateBookCommandInput { Id = "b1", ISBN = "54321" };

            await Assert.ThrowsAsync<DuplicateException>(() => service.UpdateBook(input));
        }
        [Fact]
        public async Task DeleteBook_ShouldReturnFail_WhenBookDoesNotExist()
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
        public async Task DeleteBook_ShouldReturnFail_WhenBookHasActiveLoans()
        {
            // Arrange
            var repo = A.Fake<IBookRepository>();
            var loanRepo = A.Fake<ILoanRepository>();

            var bookData = new BookData { Title = "T", AuthorId = "a", CopiesAvailable = 1, Genre = "g" };
            var book = Book.Create(bookData);
            book.Id = "book-1";

            A.CallTo(() => repo.GetById("book-1", A<CancellationToken>._)).Returns(Task.FromResult<Book?>(book));
            A.CallTo(() => loanRepo.Filter(A<LoanFilter>._, A<CancellationToken>._))
                .Returns(new List<Loan> { new Loan { Id = "loan-1", BookId = "book-1", ReaderId = "r1" } });

            var service = new BookService(repo, loanRepo);

            // Act
            var result = await service.DeleteBook("book-1");

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Book cannot be deleted while it has active loans.", result.Message);
        }

        [Fact]
        public async Task DeleteBook_ShouldDeleteAndReturnOk_WhenBookExistsAndNoActiveLoans()
        {
            // Arrange
            var repo = A.Fake<IBookRepository>();
            var loanRepo = A.Fake<ILoanRepository>();

            var bookData = new BookData { Title = "T", AuthorId = "a", CopiesAvailable = 1, Genre = "g" };
            var book = Book.Create(bookData);
            book.Id = "book-1";

            A.CallTo(() => repo.GetById("book-1", A<CancellationToken>._)).Returns(Task.FromResult<Book?>(book));
            A.CallTo(() => loanRepo.Filter(A<LoanFilter>._, A<CancellationToken>._)).Returns(new List<Loan>());
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

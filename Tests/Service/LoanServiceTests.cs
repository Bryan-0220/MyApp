using Application.Books.Services;
using UpdateLoan;
using Application.Readers.Services;
using Application.Filters;
using Application.Interfaces;
using Application.Loans.Services;
using Domain.Models;
using Domain.Common;
using FakeItEasy;

namespace Tests
{
    public class LoanServiceTests
    {
        [Fact]
        public async Task EnsureNoDuplicateLoan_ShouldThrow_WhenLoanExists()
        {
            var repo = A.Fake<ILoanRepository>();
            var bookService = A.Fake<IBookService>();
            var readerService = A.Fake<IReaderService>();
            A.CallTo(() => repo.Filter(A<LoanFilter>._, A<CancellationToken>._))
                .Returns(new[] { new Loan { Id = "l1", BookId = "b1", ReaderId = "r1" } });
            var service = new LoanService(repo, bookService, readerService);
            await Assert.ThrowsAsync<DuplicateException>(() => service.EnsureNoDuplicateLoan("b1", "r1"));
        }

        [Fact]
        public async Task EnsureNoDuplicateLoan_ShouldSucceed_WhenNoLoan()
        {
            var repo = A.Fake<ILoanRepository>();
            var bookService = A.Fake<IBookService>();
            var readerService = A.Fake<IReaderService>();
            A.CallTo(() => repo.Filter(A<LoanFilter>._, A<CancellationToken>._))
                .Returns(new List<Loan>());
            var service = new LoanService(repo, bookService, readerService);
            await service.EnsureNoDuplicateLoan("b1", "r1");
            A.CallTo(() => repo.Filter(A<LoanFilter>.That.Matches(f => f.BookId == "b1" && f.UserId == "r1"), A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CreateLoan_ShouldCreate_WhenValid()
        {
            var repo = A.Fake<ILoanRepository>();
            var bookService = A.Fake<IBookService>();
            var readerService = A.Fake<IReaderService>();
            var data = new LoanData { BookId = "b1", ReaderId = "r1", LoanDate = System.DateOnly.FromDateTime(System.DateTime.UtcNow), DueDate = System.DateOnly.FromDateTime(System.DateTime.UtcNow.AddDays(7)) };
            var book = Book.Create(new BookData { Title = "T", AuthorId = "A", CopiesAvailable = 1, Genre = "G" });
            book.Id = "b1";
            A.CallTo(() => bookService.GetBookOrThrow("b1", A<CancellationToken>._)).Returns(book);
            A.CallTo(() => readerService.EnsureExists("r1", A<CancellationToken>._)).Returns(Task.CompletedTask);
            A.CallTo(() => repo.Filter(A<LoanFilter>._, A<CancellationToken>._)).Returns(new List<Loan>());
            A.CallTo(() => bookService.DecreaseCopiesOrThrow("b1", A<CancellationToken>._)).Returns(Task.CompletedTask);
            A.CallTo(() => repo.Create(A<Loan>._, A<CancellationToken>._)).ReturnsLazily((Loan l, CancellationToken _) => Task.FromResult(l));
            var service = new LoanService(repo, bookService, readerService);
            var result = await service.CreateLoan(data);
            Assert.Equal("b1", result.BookId);
            Assert.Equal("r1", result.ReaderId);
            A.CallTo(() => repo.Create(A<Loan>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => bookService.GetBookOrThrow("b1", A<CancellationToken>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => readerService.EnsureExists("r1", A<CancellationToken>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => bookService.DecreaseCopiesOrThrow("b1", A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CreateLoan_ShouldThrowNotFound_WhenBookMissing()
        {
            var repo = A.Fake<ILoanRepository>();
            var bookService = A.Fake<IBookService>();
            var readerService = A.Fake<IReaderService>();
            var data = new LoanData { BookId = "b1", ReaderId = "r1", LoanDate = System.DateOnly.FromDateTime(System.DateTime.UtcNow), DueDate = System.DateOnly.FromDateTime(System.DateTime.UtcNow.AddDays(7)) };
            A.CallTo(() => bookService.GetBookOrThrow("b1", A<CancellationToken>._)).Throws(new NotFoundException("not found"));
            var service = new LoanService(repo, bookService, readerService);
            await Assert.ThrowsAsync<NotFoundException>(() => service.CreateLoan(data));
        }

        [Fact]
        public async Task CreateLoan_ShouldThrowDuplicate_WhenExistingLoan()
        {
            var repo = A.Fake<ILoanRepository>();
            var bookService = A.Fake<IBookService>();
            var readerService = A.Fake<IReaderService>();
            var data = new LoanData { BookId = "b1", ReaderId = "r1", LoanDate = System.DateOnly.FromDateTime(System.DateTime.UtcNow), DueDate = System.DateOnly.FromDateTime(System.DateTime.UtcNow.AddDays(7)) };
            // The service calls GetBookOrThrow and EnsureExists before checking duplicates, so fake them
            var book = Book.Create(new BookData { Title = "T", AuthorId = "A", CopiesAvailable = 1, Genre = "G" });
            book.Id = "b1";
            A.CallTo(() => bookService.GetBookOrThrow("b1", A<CancellationToken>._)).Returns(book);
            A.CallTo(() => readerService.EnsureExists("r1", A<CancellationToken>._)).Returns(Task.CompletedTask);
            A.CallTo(() => repo.Filter(A<LoanFilter>._, A<CancellationToken>._)).Returns(new[] { new Loan() });
            var service = new LoanService(repo, bookService, readerService);
            await Assert.ThrowsAsync<DuplicateException>(() => service.CreateLoan(data));
        }

        [Fact]
        public async Task CreateLoan_ShouldThrowBusinessRule_WhenNoCopies()
        {
            var repo = A.Fake<ILoanRepository>();
            var bookService = A.Fake<IBookService>();
            var readerService = A.Fake<IReaderService>();
            var data = new LoanData { BookId = "b1", ReaderId = "r1", LoanDate = System.DateOnly.FromDateTime(System.DateTime.UtcNow), DueDate = System.DateOnly.FromDateTime(System.DateTime.UtcNow.AddDays(7)) };
            var book = Book.Create(new BookData { Title = "T", AuthorId = "A", CopiesAvailable = 1, Genre = "G" });
            book.Id = "b1";
            A.CallTo(() => bookService.GetBookOrThrow("b1", A<CancellationToken>._)).Returns(book);
            A.CallTo(() => readerService.EnsureExists("r1", A<CancellationToken>._)).Returns(Task.CompletedTask);
            A.CallTo(() => repo.Filter(A<LoanFilter>._, A<CancellationToken>._)).Returns(new List<Loan>());
            A.CallTo(() => bookService.DecreaseCopiesOrThrow("b1", A<CancellationToken>._)).Throws(new BusinessRuleException("no copies"));
            var service = new LoanService(repo, bookService, readerService);
            await Assert.ThrowsAsync<BusinessRuleException>(() => service.CreateLoan(data));
        }

        [Fact]
        public async Task UpdateLoan_ShouldUpdate_WhenExists()
        {
            var repo = A.Fake<ILoanRepository>();
            var bookService = A.Fake<IBookService>();
            var readerService = A.Fake<IReaderService>();
            var loan = new Loan { Id = "l1", BookId = "b1", ReaderId = "r1" };
            A.CallTo(() => repo.GetById("l1", A<CancellationToken>._)).Returns(Task.FromResult<Loan?>(loan));
            A.CallTo(() => repo.Update(loan, A<CancellationToken>._)).Returns(Task.FromResult(true));
            // When updating BookId, the service validates target book and reader exist
            var targetBook = Book.Create(new BookData { Title = "T2", AuthorId = "A", CopiesAvailable = 1, Genre = "G" });
            targetBook.Id = "b2";
            A.CallTo(() => bookService.GetBookOrThrow("b2", A<CancellationToken>._)).Returns(targetBook);
            A.CallTo(() => readerService.EnsureExists("r1", A<CancellationToken>._)).Returns(Task.CompletedTask);

            var service = new LoanService(repo, bookService, readerService);
            var input = new UpdateLoanCommandInput { Id = "l1", BookId = "b2" };
            var updated = await service.UpdateLoan(input);
            Assert.Equal("b2", updated.BookId);
            A.CallTo(() => repo.Update(A<Loan>.That.Matches(l => l.Id == "l1" && l.BookId == "b2"), A<CancellationToken>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => bookService.GetBookOrThrow("b2", A<CancellationToken>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => readerService.EnsureExists("r1", A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task UpdateLoan_ShouldThrowNotFound_WhenMissing()
        {
            var repo = A.Fake<ILoanRepository>();
            var bookService = A.Fake<IBookService>();
            var readerService = A.Fake<IReaderService>();
            A.CallTo(() => repo.GetById("l2", A<CancellationToken>._)).Returns(Task.FromResult<Loan?>(null));
            var service = new LoanService(repo, bookService, readerService);
            var input = new UpdateLoanCommandInput { Id = "l2", BookId = "b2" };
            await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateLoan(input));
        }

        [Fact]
        public async Task DeleteLoan_ShouldReturnFail_WhenLoanIsActiveOrOverdue()
        {
            var repo = A.Fake<ILoanRepository>();
            var bookService = A.Fake<IBookService>();
            var readerService = A.Fake<IReaderService>();
            var loan = new Loan { Id = "l3", BookId = "b3", ReaderId = "r3", Status = LoanStatus.Active };
            A.CallTo(() => repo.GetById("l3", A<CancellationToken>._)).Returns(Task.FromResult<Loan?>(loan));
            var service = new LoanService(repo, bookService, readerService);
            var result = await service.DeleteLoan("l3");
            Assert.False(result.Success);
            Assert.Equal("Cannot delete an active or overdue loan. Mark it as returned before deletion.", result.Message);
            A.CallTo(() => bookService.RestoreCopies(A<string>._, A<CancellationToken>._)).MustNotHaveHappened();
            A.CallTo(() => repo.Delete(A<string>._, A<CancellationToken>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task DeleteLoan_ShouldDeleteAndRestoreCopies_WhenLoanIsReturned()
        {
            var repo = A.Fake<ILoanRepository>();
            var bookService = A.Fake<IBookService>();
            var readerService = A.Fake<IReaderService>();
            var loan = new Loan { Id = "l3", BookId = "b3", ReaderId = "r3", Status = LoanStatus.Returned };
            A.CallTo(() => repo.GetById("l3", A<CancellationToken>._)).Returns(Task.FromResult<Loan?>(loan));
            A.CallTo(() => bookService.RestoreCopies(A<string>._, A<CancellationToken>._)).Returns(Task.CompletedTask);
            A.CallTo(() => repo.Delete("l3", A<CancellationToken>._)).Returns(Task.FromResult(true));
            var service = new LoanService(repo, bookService, readerService);
            var result = await service.DeleteLoan("l3");
            Assert.True(result.Success);
            Assert.Equal("Loan deleted.", result.Message);
            A.CallTo(() => bookService.RestoreCopies("b3", A<CancellationToken>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => repo.Delete("l3", A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task DeleteLoan_ShouldReturnFail_WhenRestoreCopiesFails()
        {
            var repo = A.Fake<ILoanRepository>();
            var bookService = A.Fake<IBookService>();
            var readerService = A.Fake<IReaderService>();
            var loan = new Loan { Id = "l4", BookId = "b4", ReaderId = "r4", Status = LoanStatus.Returned };
            A.CallTo(() => repo.GetById("l4", A<CancellationToken>._)).Returns(Task.FromResult<Loan?>(loan));

            A.CallTo(() => bookService.RestoreCopies("b4", A<CancellationToken>._)).Throws(new BusinessRuleException("restore fail"));
            var service = new LoanService(repo, bookService, readerService);
            var result = await service.DeleteLoan("l4");
            Assert.False(result.Success);
            Assert.Equal("restore fail", result.Message);

            A.CallTo(() => repo.Delete("l4", A<CancellationToken>._)).MustNotHaveHappened();
        }

    }
}

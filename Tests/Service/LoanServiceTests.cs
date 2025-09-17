using Application.Books.Services;
using Application.Interfaces;
using Application.Loans.Services;
using Domain.Models;
using FakeItEasy;

namespace Tests
{
    public class LoanServiceTests
    {
        [Fact]
        public async Task DeleteLoan_ShouldReturnFail_WhenLoanDoesNotExist()
        {
            // Arrange
            var repo = A.Fake<ILoanRepository>();
            var bookService = A.Fake<IBookService>();
            var readerService = A.Fake<Application.Readers.Services.IReaderService>();

            A.CallTo(() => repo.GetById(A<string>._, A<CancellationToken>._)).Returns(Task.FromResult<Loan?>(null));

            var service = new LoanService(repo, bookService, readerService);

            // Act
            var result = await service.DeleteLoan("non-existent-id");

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Loan not found.", result.Message);
        }

        [Fact]
        public async Task DeleteLoan_ShouldDeleteAndReturnOk_WhenLoanExists()
        {
            // Arrange
            var repo = A.Fake<ILoanRepository>();
            var bookService = A.Fake<IBookService>();

            var readerService = A.Fake<Application.Readers.Services.IReaderService>();

            var loan = new Loan { Id = "loan-1", BookId = "book-1", ReaderId = "reader-1", LoanDate = System.DateOnly.FromDateTime(System.DateTime.UtcNow), DueDate = System.DateOnly.FromDateTime(System.DateTime.UtcNow.AddDays(7)), Status = LoanStatus.Returned };

            A.CallTo(() => repo.GetById("loan-1", A<CancellationToken>._)).Returns(Task.FromResult<Loan?>(loan));
            A.CallTo(() => repo.Delete("loan-1", A<CancellationToken>._)).Returns(Task.FromResult(true));
            A.CallTo(() => bookService.RestoreCopies(A<string>._, A<CancellationToken>._)).Returns(Task.CompletedTask);

            var service = new LoanService(repo, bookService, readerService);

            // Act
            var result = await service.DeleteLoan("loan-1");

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Loan deleted.", result.Message);
            Assert.Equal("loan-1", result.Value?.Id);

            A.CallTo(() => repo.Delete("loan-1", A<CancellationToken>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => bookService.RestoreCopies("book-1", A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        }
    }
}

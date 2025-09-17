using Application.Readers.Services;
using Application.Interfaces;
using Domain.Models;
using FakeItEasy;

namespace Tests
{
    public class ReaderServiceTests
    {
        [Fact]
        public async Task DeleteReader_ShouldReturnFail_WhenReaderDoesNotExist()
        {
            // Arrange
            var readerRepo = A.Fake<IReaderRepository>();
            var loanRepo = A.Fake<ILoanRepository>();

            A.CallTo(() => readerRepo.GetById(A<string>._, A<CancellationToken>._)).Returns(Task.FromResult<Reader?>(null));

            var service = new ReaderService(readerRepo, loanRepo);

            // Act
            var result = await service.DeleteReader("non-existent-id");

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Reader not found.", result.Message);
        }

        [Fact]
        public async Task DeleteReader_ShouldReturnFail_WhenReaderHasActiveLoans()
        {
            // Arrange
            var readerRepo = A.Fake<IReaderRepository>();
            var loanRepo = A.Fake<ILoanRepository>();

            var readerData = new ReaderData { FirstName = "F", LastName = "L", Email = "e@x.com", MembershipDate = DateOnly.FromDateTime(System.DateTime.UtcNow) };
            var reader = Reader.Create(readerData);
            reader.Id = "reader-1";

            A.CallTo(() => readerRepo.GetById("reader-1", A<CancellationToken>._)).Returns(Task.FromResult<Reader?>(reader));
            A.CallTo(() => loanRepo.Filter(A<Application.Filters.LoanFilter>._, A<CancellationToken>._))
                .Returns(new List<Loan> { new Loan { Id = "loan-1", BookId = "b1", ReaderId = "reader-1" } });

            var service = new ReaderService(readerRepo, loanRepo);

            // Act
            var result = await service.DeleteReader("reader-1");

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Reader cannot be deleted while has active loans.", result.Message);
        }

        [Fact]
        public async Task DeleteReader_ShouldDeleteAndReturnOk_WhenReaderExistsAndNoActiveLoans()
        {
            // Arrange
            var readerRepo = A.Fake<IReaderRepository>();
            var loanRepo = A.Fake<ILoanRepository>();

            var readerData = new ReaderData { FirstName = "F", LastName = "L", Email = "e@x.com", MembershipDate = DateOnly.FromDateTime(System.DateTime.UtcNow) };
            var reader = Reader.Create(readerData);
            reader.Id = "reader-1";

            A.CallTo(() => readerRepo.GetById("reader-1", A<CancellationToken>._)).Returns(Task.FromResult<Reader?>(reader));
            A.CallTo(() => loanRepo.Filter(A<Application.Filters.LoanFilter>._, A<CancellationToken>._)).Returns(new List<Loan>());
            A.CallTo(() => readerRepo.Delete("reader-1", A<CancellationToken>._)).Returns(Task.FromResult(true));

            var service = new ReaderService(readerRepo, loanRepo);

            // Act
            var result = await service.DeleteReader("reader-1");

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Reader deleted.", result.Message);
            Assert.Equal("reader-1", result.Value?.Id);

            A.CallTo(() => readerRepo.Delete("reader-1", A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        }
    }
}

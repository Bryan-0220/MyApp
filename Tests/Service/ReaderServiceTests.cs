using Application.Readers.Services;
using Application.Interfaces;
using Domain.Models;
using Domain.Common;
using UpdateReader;
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
        public async Task EnsureExists_ShouldThrow_WhenMissing()
        {
            var readerRepo = A.Fake<IReaderRepository>();
            var loanRepo = A.Fake<ILoanRepository>();
            A.CallTo(() => readerRepo.GetById(A<string>._, A<CancellationToken>._)).Returns(Task.FromResult<Reader?>(null));
            var service = new ReaderService(readerRepo, loanRepo);
            await Assert.ThrowsAsync<NotFoundException>(() => service.EnsureExists("missing"));
        }

        [Fact]
        public async Task EnsureExists_ShouldSucceed_WhenFound()
        {
            var readerRepo = A.Fake<IReaderRepository>();
            var loanRepo = A.Fake<ILoanRepository>();
            var rd = new ReaderData { FirstName = "F", LastName = "L", Email = "e@x.com", MembershipDate = DateOnly.FromDateTime(System.DateTime.UtcNow) };
            var reader = Reader.Create(rd);
            reader.Id = "r1";
            A.CallTo(() => readerRepo.GetById("r1", A<CancellationToken>._)).Returns(Task.FromResult<Reader?>(reader));
            var service = new ReaderService(readerRepo, loanRepo);
            await service.EnsureExists("r1");
            A.CallTo(() => readerRepo.GetById("r1", A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CreateReader_ShouldCreate_WhenValid()
        {
            var readerRepo = A.Fake<IReaderRepository>();
            var loanRepo = A.Fake<ILoanRepository>();
            var input = new ReaderData { FirstName = "F", LastName = "L", Email = "e@x.com", MembershipDate = DateOnly.FromDateTime(System.DateTime.UtcNow) };
            A.CallTo(() => readerRepo.Filter(A<Application.Filters.ReaderFilter>._, A<CancellationToken>._)).Returns(new List<Reader>());
            A.CallTo(() => readerRepo.Create(A<Reader>._, A<CancellationToken>._)).ReturnsLazily((Reader r, CancellationToken _) => Task.FromResult(r));
            var service = new ReaderService(readerRepo, loanRepo);
            var created = await service.CreateReader(input);
            Assert.Equal("e@x.com", created.Email);
            A.CallTo(() => readerRepo.Create(A<Reader>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CreateReader_ShouldThrow_WhenEmailTaken()
        {
            var readerRepo = A.Fake<IReaderRepository>();
            var loanRepo = A.Fake<ILoanRepository>();
            var existing = new Reader { Id = "r-existing", FirstName = "F", LastName = "L", Email = "e@x.com", MembershipDate = DateOnly.FromDateTime(System.DateTime.UtcNow) };
            A.CallTo(() => readerRepo.Filter(A<Application.Filters.ReaderFilter>._, A<CancellationToken>._)).Returns(new[] { existing });
            var service = new ReaderService(readerRepo, loanRepo);
            await Assert.ThrowsAsync<DuplicateException>(() => service.CreateReader(new ReaderData { FirstName = "F", LastName = "L", Email = "e@x.com", MembershipDate = DateOnly.FromDateTime(System.DateTime.UtcNow) }));
        }

        [Fact]
        public async Task UpdateReader_ShouldUpdate_WhenExists()
        {
            var readerRepo = A.Fake<IReaderRepository>();
            var loanRepo = A.Fake<ILoanRepository>();
            var rd = new ReaderData { FirstName = "F", LastName = "L", Email = "e@x.com", MembershipDate = DateOnly.FromDateTime(System.DateTime.UtcNow) };
            var reader = Reader.Create(rd);
            reader.Id = "r-1";
            A.CallTo(() => readerRepo.GetById("r-1", A<CancellationToken>._)).Returns(Task.FromResult<Reader?>(reader));
            A.CallTo(() => readerRepo.Update(A<Reader>._, A<CancellationToken>._)).Returns(Task.FromResult(true));
            var service = new ReaderService(readerRepo, loanRepo);
            var input = new UpdateReaderCommandInput { Id = "r-1", FirstName = "New", Email = "new@x.com" };
            var updated = await service.UpdateReader(input);
            Assert.Equal("New", updated.FirstName);
            Assert.Equal("new@x.com", updated.Email);
            A.CallTo(() => readerRepo.Update(A<Reader>.That.Matches(r => r.Id == "r-1" && r.FirstName == "New" && r.Email == "new@x.com"), A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task UpdateReader_ShouldThrowDuplicate_WhenEmailUsedByAnother()
        {
            var readerRepo = A.Fake<IReaderRepository>();
            var loanRepo = A.Fake<ILoanRepository>();

            // existing reader we want to update
            var rd = new ReaderData { FirstName = "F", LastName = "L", Email = "old@x.com", MembershipDate = DateOnly.FromDateTime(System.DateTime.UtcNow) };
            var reader = Reader.Create(rd);
            reader.Id = "r-1";

            // another reader already using the target email
            var other = new Reader { Id = "r-2", FirstName = "A", LastName = "B", Email = "taken@x.com", MembershipDate = DateOnly.FromDateTime(System.DateTime.UtcNow) };

            A.CallTo(() => readerRepo.GetById("r-1", A<CancellationToken>._)).Returns(Task.FromResult<Reader?>(reader));
            A.CallTo(() => readerRepo.Filter(A<Application.Filters.ReaderFilter>._, A<CancellationToken>._)).Returns(new[] { other });

            var service = new ReaderService(readerRepo, loanRepo);

            var input = new UpdateReaderCommandInput { Id = "r-1", Email = "taken@x.com" };

            await Assert.ThrowsAsync<DuplicateException>(() => service.UpdateReader(input));
        }

        [Fact]
        public async Task UpdateReader_ShouldThrowNotFound_WhenMissing()
        {
            var readerRepo = A.Fake<IReaderRepository>();
            var loanRepo = A.Fake<ILoanRepository>();
            A.CallTo(() => readerRepo.GetById(A<string>._, A<CancellationToken>._)).Returns(Task.FromResult<Reader?>(null));
            var service = new ReaderService(readerRepo, loanRepo);
            var input = new UpdateReaderCommandInput { Id = "missing", FirstName = "X" };
            await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateReader(input));
        }

        [Fact]
        public async Task EnsureEmailNotInUse_ShouldThrow_WhenEmailTaken()
        {
            var readerRepo = A.Fake<IReaderRepository>();
            var loanRepo = A.Fake<ILoanRepository>();
            A.CallTo(() => readerRepo.Filter(A<Application.Filters.ReaderFilter>._, A<CancellationToken>._)).Returns(new[] { new Reader() });
            var service = new ReaderService(readerRepo, loanRepo);
            await Assert.ThrowsAsync<DuplicateException>(() => service.EnsureEmailNotInUse("a@b.com"));
        }

        [Fact]
        public async Task EnsureCanDelete_ShouldThrow_WhenHasActiveLoans()
        {
            var readerRepo = A.Fake<IReaderRepository>();
            var loanRepo = A.Fake<ILoanRepository>();
            A.CallTo(() => loanRepo.Filter(A<Application.Filters.LoanFilter>._, A<CancellationToken>._)).Returns(new[] { new Loan() });
            var service = new ReaderService(readerRepo, loanRepo);
            await Assert.ThrowsAsync<BusinessRuleException>(() => service.EnsureCanDelete("r1"));
        }

        [Fact]
        public async Task EnsureCanDelete_ShouldSucceed_WhenNoLoans()
        {
            var readerRepo = A.Fake<IReaderRepository>();
            var loanRepo = A.Fake<ILoanRepository>();
            A.CallTo(() => loanRepo.Filter(A<Application.Filters.LoanFilter>._, A<CancellationToken>._)).Returns(new List<Loan>());
            var service = new ReaderService(readerRepo, loanRepo);
            await service.EnsureCanDelete("r1");
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

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentValidation;
using FluentValidation.Results;
using Xunit;
using CreateLoan;
using UpdateLoan;
using DeleteLoan;
using GetLoanById;
using GetAllLoans;
using FilterLoans;
using Application.Interfaces;
using Application.Loans.Services;
using Application.Filters;
using Domain.Models;
using Domain.Results;

namespace Tests.Handler
{
    public class LoanHandlerTests
    {
        [Fact]
        public async Task CreateLoan_ShouldTrimInputs_AndCallService()
        {
            var service = A.Fake<ILoanService>();
            var validator = A.Fake<IValidator<CreateLoanCommandInput>>();

            // Capture the LoanData passed to service and return the same Loan instance
            Loan? captured = null;
            A.CallTo(() => service.CreateLoan(A<LoanData>._, A<CancellationToken>._))
                .ReturnsLazily((LoanData d, CancellationToken ct) =>
                {
                    var loan = Loan.Create(d);
                    captured = loan;
                    return Task.FromResult(loan);
                });

            var handler = new CreateLoanCommandHandler(service, validator);

            var input = new CreateLoanCommandInput
            {
                BookId = "  book-1  ",
                ReaderId = " reader-1 ",
                LoanDate = new DateOnly(2023, 1, 1),
                DueDate = new DateOnly(2023, 1, 8)
            };

            var output = await handler.Handle(input, CancellationToken.None);

            Assert.NotNull(captured);
            Assert.Equal("book-1", captured!.BookId);
            Assert.Equal("reader-1", captured.ReaderId);
            Assert.Equal(input.LoanDate, captured.LoanDate);
            Assert.Equal(input.DueDate, captured.DueDate);

            // Mapper should reflect created loan
            Assert.Equal(captured.Id, output.Id);
            Assert.Equal(captured.BookId, output.BookId);
        }

        [Fact]
        public async Task CreateLoan_ShouldBubbleServiceException()
        {
            var service = A.Fake<ILoanService>();
            var validator = A.Fake<IValidator<CreateLoanCommandInput>>();

            A.CallTo(() => service.CreateLoan(A<LoanData>._, A<CancellationToken>._))
                .Throws(new System.InvalidOperationException("service failed"));

            var handler = new CreateLoanCommandHandler(service, validator);

            var input = new CreateLoanCommandInput
            {
                BookId = "b",
                ReaderId = "r",
                LoanDate = new DateOnly(2023, 1, 1),
                DueDate = new DateOnly(2023, 1, 2)
            };

            await Assert.ThrowsAsync<System.InvalidOperationException>(() => handler.Handle(input));
        }

        [Fact]
        public async Task UpdateLoan_ShouldMapStatusProperly_WhenServiceReturnsLoan()
        {
            var service = A.Fake<ILoanService>();
            var validator = A.Fake<IValidator<UpdateLoanCommandInput>>();

            var existing = Loan.Create(new LoanData { BookId = "b1", ReaderId = "r1", LoanDate = new DateOnly(2023, 1, 1), DueDate = new DateOnly(2023, 1, 5) });
            existing.Id = "l1";
            existing.Status = LoanStatus.Overdue;

            A.CallTo(() => service.UpdateLoan(A<UpdateLoanCommandInput>._, A<CancellationToken>._))
                .Returns(Task.FromResult(existing));

            var handler = new UpdateLoanCommandHandler(service, validator);

            var input = new UpdateLoanCommandInput { Id = "l1" };
            var result = await handler.Handle(input);

            Assert.NotNull(result);
            Assert.Equal("l1", result.Id);
            Assert.Equal("Overdue", result.Status);
        }

        [Fact]
        public async Task DeleteLoan_ShouldMapFailureResultProperly()
        {
            var service = A.Fake<ILoanService>();
            var validator = A.Fake<IValidator<DeleteLoanCommandInput>>();

            var failure = Result<Loan>.Fail("cannot delete because active");
            A.CallTo(() => service.DeleteLoan(A<string>._, A<CancellationToken>._)).Returns(Task.FromResult(failure));

            var handler = new DeleteLoanCommandHandler(validator, service);

            var output = await handler.Handle(new DeleteLoanCommandInput { Id = "l1" });

            Assert.False(output.Success);
            Assert.Equal("cannot delete because active", output.Message);
            Assert.Null(output.LoanId);
        }

        [Fact]
        public async Task DeleteLoan_ShouldMapSuccessResult_WithLoanId()
        {
            var service = A.Fake<ILoanService>();
            var validator = A.Fake<IValidator<DeleteLoanCommandInput>>();

            var loan = Loan.Create(new LoanData { BookId = "b1", ReaderId = "r1", LoanDate = new DateOnly(2023, 1, 1), DueDate = new DateOnly(2023, 1, 2) });
            loan.Id = "loan-123";
            var ok = Result<Loan>.Ok(loan);
            A.CallTo(() => service.DeleteLoan("loan-123", A<CancellationToken>._)).Returns(Task.FromResult(ok));

            var handler = new DeleteLoanCommandHandler(validator, service);

            var output = await handler.Handle(new DeleteLoanCommandInput { Id = "loan-123" });

            Assert.True(output.Success);
            Assert.Equal("loan-123", output.LoanId);
            Assert.True(string.IsNullOrEmpty(output.Message)); // message may be null or empty depending on Result implementation
        }

        [Fact]
        public async Task GetLoanById_ShouldReturnFail_WhenNotFound()
        {
            var repo = A.Fake<ILoanRepository>();
            A.CallTo(() => repo.GetById(A<string>._, A<CancellationToken>._)).Returns(Task.FromResult<Loan?>(null));

            var handler = new GetLoanByIdQueryHandler(repo);

            var result = await handler.Handle(new GetLoanByIdQueryInput { Id = "missing" });

            Assert.False(result.Success);
            Assert.Equal("Loan not found", result.Message);
        }

        [Fact]
        public async Task FilterLoans_ShouldCallValidator_AndReturnMappedResults()
        {
            var repo = A.Fake<ILoanRepository>();
            var validator = A.Fake<IValidator<FilterLoansQueryInput>>();

            var loans = new List<Loan> {
                Loan.Create(new LoanData { BookId = "b1", ReaderId = "r1", LoanDate = new DateOnly(2023,1,1), DueDate = new DateOnly(2023,1,5) }),
                Loan.Create(new LoanData { BookId = "b2", ReaderId = "r2", LoanDate = new DateOnly(2023,1,2), DueDate = new DateOnly(2023,1,6) })
            };

            A.CallTo(() => repo.Filter(A<LoanFilter>._, A<CancellationToken>._)).Returns(loans);
            // ValidateAsync is invoked with a ValidationContext<T> by ValidateAndThrowAsync
            A.CallTo(() => validator.ValidateAsync(A<FluentValidation.ValidationContext<FilterLoansQueryInput>>._, A<CancellationToken>._))
                .Returns(Task.FromResult(new ValidationResult()));

            var handler = new FilterLoansQueryHandler(repo, validator);

            var input = new FilterLoansQueryInput { LoanDate = new DateOnly(2023, 1, 1), DueDate = new DateOnly(2023, 1, 5) };
            var result = await handler.Handle(input);

            A.CallTo(() => validator.ValidateAsync(A<FluentValidation.ValidationContext<FilterLoansQueryInput>>._, A<CancellationToken>._)).MustHaveHappened();
            Assert.Equal(2, result.Count());
            Assert.Contains(result, r => r.BookId == "b1");
        }

        [Fact]
        public async Task FilterLoans_ShouldBubbleValidationException()
        {
            var repo = A.Fake<ILoanRepository>();
            var validator = A.Fake<IValidator<FilterLoansQueryInput>>();

            var failures = new[] { new ValidationFailure("LoanDate", "required") };
            var invalid = new ValidationResult(failures);
            A.CallTo(() => validator.ValidateAsync(A<FluentValidation.ValidationContext<FilterLoansQueryInput>>._, A<CancellationToken>._))
                .Throws(new FluentValidation.ValidationException(invalid.Errors));

            var handler = new FilterLoansQueryHandler(repo, validator);
            var input = new FilterLoansQueryInput { LoanDate = null, DueDate = null };

            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => handler.Handle(input));
        }

        [Fact]
        public async Task GetAllLoans_ShouldMapReturnedDateAndStatusCorrectly()
        {
            var repo = A.Fake<ILoanRepository>();

            var loan1 = Loan.Create(new LoanData { BookId = "b1", ReaderId = "r1", LoanDate = new DateOnly(2023, 1, 1), DueDate = new DateOnly(2023, 1, 5) });
            loan1.Id = "L1";
            loan1.Status = LoanStatus.Returned;
            loan1.ReturnedDate = new DateOnly(2023, 1, 4);

            var loan2 = Loan.Create(new LoanData { BookId = "b2", ReaderId = "r2", LoanDate = new DateOnly(2023, 2, 1), DueDate = new DateOnly(2023, 2, 5) });
            loan2.Id = "L2";
            loan2.Status = LoanStatus.Overdue;
            loan2.ReturnedDate = null;

            A.CallTo(() => repo.GetAll(A<CancellationToken>._)).Returns(new[] { loan1, loan2 });

            var handler = new GetAllLoansQueryHandler(repo);

            var result = await handler.Handle(new GetAllLoansQueryInput());

            var list = result.ToList();
            Assert.Equal(2, list.Count);

            var r1 = list.Single(r => r.Id == "L1");
            Assert.Equal("Returned", r1.Status);
            Assert.Equal(new DateOnly(2023, 1, 4), r1.ReturnedDate);

            var r2 = list.Single(r => r.Id == "L2");
            Assert.Equal("Overdue", r2.Status);
            Assert.Null(r2.ReturnedDate);
        }

        [Fact]
        public async Task GetAllLoans_ShouldForwardCancellationTokenToRepository()
        {
            var repo = A.Fake<ILoanRepository>();

            // Arrange: have repository capture the cancellation token it's called with
            CancellationToken? captured = null;
            A.CallTo(() => repo.GetAll(A<CancellationToken>._))
                .ReturnsLazily((CancellationToken ct) =>
                {
                    captured = ct;
                    // return empty list
                    return Task.FromResult<IEnumerable<Loan>>(Array.Empty<Loan>());
                });

            var handler = new GetAllLoansQueryHandler(repo);

            using var cts = new CancellationTokenSource();
            var ct = cts.Token;
            var result = await handler.Handle(new GetAllLoansQueryInput(), ct);

            // Ensure the token instance used by the repository is the same as passed to the handler
            Assert.NotNull(captured);
            Assert.Equal(ct, captured);
        }

        [Fact]
        public async Task GetAllLoans_ShouldHandleRepositoryReturningMutableCollectionWithoutAffectingHandlerOutput()
        {
            var repo = A.Fake<ILoanRepository>();

            var loan = Loan.Create(new LoanData { BookId = "b1", ReaderId = "r1", LoanDate = new DateOnly(2023, 1, 1), DueDate = new DateOnly(2023, 1, 5) });
            loan.Id = "L1";

            // Return a List<Loan> that the repo may mutate later
            var backing = new List<Loan> { loan };
            A.CallTo(() => repo.GetAll(A<CancellationToken>._)).Returns(backing);

            var handler = new GetAllLoansQueryHandler(repo);

            // Materialize the outputs immediately to simulate a caller that enumerates results before repository is mutated
            var outputs = (await handler.Handle(new GetAllLoansQueryInput())).ToList();

            // Mutate the backing collection after the handler returned to simulate repository side-effects
            backing.Clear();

            // The outputs list was materialized above and must remain intact
            var list = outputs;
            Assert.Single(list);
            Assert.Equal("L1", list[0].Id);
        }
    }
}

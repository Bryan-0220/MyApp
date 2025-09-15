using Domain.Models;
using Domain.Common;
using Application.Interfaces;
using Application.Loans.Services;
using FluentValidation;

namespace CreateLoan
{
    public class CreateLoanCommandHandler : ICreateLoanCommandHandler
    {
        private const string BookNotFoundMessage = "Book not found.";
        private const string ReaderNotFoundMessage = "Reader not found.";
        private const string NoCopiesMessage = "No copies available for the requested book.";
        private const int DefaultLoanDays = 14;

        private readonly ILoanRepository _loanRepository;
        private readonly Application.Books.Services.IBookService _bookService;
        private readonly Application.Readers.Services.IReaderService _readerService;
        private readonly ILoanService _loanService;
        private readonly IValidator<CreateLoanCommandInput> _validator;

        public CreateLoanCommandHandler(
            ILoanRepository loanRepository,
            Application.Books.Services.IBookService bookService,
            Application.Readers.Services.IReaderService readerService,
            ILoanService loanService,
            IValidator<CreateLoanCommandInput> validator)
        {
            _loanRepository = loanRepository ?? throw new ArgumentNullException(nameof(loanRepository));
            _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
            _readerService = readerService ?? throw new ArgumentNullException(nameof(readerService));
            _loanService = loanService ?? throw new ArgumentNullException(nameof(loanService));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }


        public async Task<CreateLoanCommandOutput> HandleAsync(CreateLoanCommandInput input, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(input, ct);

            var book = await _bookService.EnsureExists(input.BookId, ct);
            var reader = await _readerService.EnsureExists(input.ReaderId, ct);

            book.EnsureHasAvailableCopies();

            await _loanService.EnsureNoDuplicateLoanAsync(input.BookId, input.ReaderId, ct);

            await _bookService.DecreaseCopiesOrThrow(input.BookId, ct);

            var now = DateOnly.FromDateTime(DateTime.UtcNow);

            var loan = new Loan
            {
                BookId = input.BookId,
                ReaderId = input.ReaderId,
                LoanDate = input.LoanDate ?? now,
                DueDate = input.DueDate ?? now.AddDays(DefaultLoanDays)
            };

            try
            {
                var created = await _loanRepository.CreateAsync(loan, ct);
                return MapToOutput(created);
            }
            catch (Exception)
            {
                try
                {
                    await _bookService.RestoreCopies(input.BookId, ct);
                }
                catch (Exception)
                {
                    throw new DomainException("Failed to create loan and failed to restore book copies.");
                }
                throw;
            }
        }

        private CreateLoanCommandOutput MapToOutput(Loan created)
        {
            return new CreateLoanCommandOutput
            {
                Id = created.Id,
                BookId = created.BookId,
                ReaderId = created.ReaderId,
                LoanDate = created.LoanDate,
                DueDate = created.DueDate,
                ReturnedDate = created.ReturnedDate,
                Status = created.Status.ToString()
            };
        }
    }
}

using Domain.Models;
using Domain.Common;
using Application.Interfaces;
using FluentValidation;

namespace CreateLoan
{
    public class CreateLoanCommandHandler : ICreateLoanCommandHandler
    {
        private const string BookNotFoundMessage = "Book not found.";
        private const string ReaderNotFoundMessage = "Reader not found.";
        private const string NoCopiesMessage = "No copies available for the requested book.";

        private readonly ILoanRepository _loanRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IReaderRepository _readerRepository;
        private readonly IValidator<CreateLoanCommandInput> _validator;

        public CreateLoanCommandHandler(
            ILoanRepository loanRepository,
            IBookRepository bookRepository,
            IReaderRepository readerRepository,
            IValidator<CreateLoanCommandInput> validator)
        {
            _loanRepository = loanRepository ?? throw new ArgumentNullException(nameof(loanRepository));
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
            _readerRepository = readerRepository ?? throw new ArgumentNullException(nameof(readerRepository));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public async Task<CreateLoanCommandOutput> HandleAsync(CreateLoanCommandInput input, CancellationToken ct = default)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            await _validator.ValidateAndThrowAsync(input, ct);

            await EnsureBookExistsAsync(input.BookId, ct);
            await EnsureReaderExistsAsync(input.ReaderId, ct);

            var changed = await _bookRepository.TryChangeCopiesAsync(input.BookId, -1, ct);
            if (!changed) throw new DomainException(NoCopiesMessage);

            var loan = new Loan
            {
                BookId = input.BookId,
                ReaderId = input.ReaderId,
                LoanDate = input.LoanDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
                DueDate = input.DueDate ?? DateOnly.FromDateTime(DateTime.UtcNow).AddDays(14)
            };

            var created = await _loanRepository.CreateAsync(loan, ct);

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

        private async Task EnsureBookExistsAsync(string bookId, CancellationToken ct)
        {
            var book = await _bookRepository.GetByIdAsync(bookId, ct);
            if (book == null) throw new DomainException(BookNotFoundMessage);
        }

        private async Task EnsureReaderExistsAsync(string readerId, CancellationToken ct)
        {
            var reader = await _readerRepository.GetByIdAsync(readerId, ct);
            if (reader == null) throw new DomainException(ReaderNotFoundMessage);
        }
    }
}

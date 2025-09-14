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
        private const int DefaultLoanDays = 14;

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
            // LoanService business validation moved to domain entity Book.EnsureHasAvailableCopies()
        }

        public async Task<CreateLoanCommandOutput> HandleAsync(CreateLoanCommandInput input, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(input, ct);

            var book = await EnsureBookExistsAsync(input.BookId, ct);
            var reader = await EnsureReaderExistsAsync(input.ReaderId, ct);

            // Validate via domain entity
            book.EnsureHasAvailableCopies();

            var changed = await _bookRepository.TryChangeCopiesAsync(input.BookId, -1, ct);
            if (!changed) throw new DomainException(NoCopiesMessage);

            var now = DateOnly.FromDateTime(DateTime.UtcNow);

            var loan = new Loan
            {
                BookId = input.BookId,
                ReaderId = input.ReaderId,
                LoanDate = input.LoanDate ?? now,
                DueDate = input.DueDate ?? now.AddDays(DefaultLoanDays)
            };

            var created = await _loanRepository.CreateAsync(loan, ct);

            return MapToOutput(created);
        }

        private async Task<Book> EnsureBookExistsAsync(string bookId, CancellationToken ct)
        {
            var book = await _bookRepository.GetByIdAsync(bookId, ct);
            if (book == null) throw new DomainException(BookNotFoundMessage);
            return book;
        }

        private async Task<Domain.Models.Reader> EnsureReaderExistsAsync(string readerId, CancellationToken ct)
        {
            var reader = await _readerRepository.GetByIdAsync(readerId, ct);
            if (reader == null) throw new DomainException(ReaderNotFoundMessage);
            return reader;
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

using Application.Interfaces;
using Application.Loans.Mappers;
using Application.Loans.Services;
using Application.Readers.Services;
using Application.Books.Services;
using Domain.Common;
using Domain.Models;
using FluentValidation;

namespace CreateLoan
{
    public class CreateLoanCommandHandler : ICreateLoanCommandHandler
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IBookService _bookService;
        private readonly IReaderService _readerService;
        private readonly ILoanService _loanService;
        private readonly IValidator<CreateLoanCommandInput> _validator;

        public CreateLoanCommandHandler(
            ILoanRepository loanRepository,
            IBookService bookService,
            IReaderService readerService,
            ILoanService loanService,
            IValidator<CreateLoanCommandInput> validator)
        {
            _loanRepository = loanRepository ?? throw new ArgumentNullException(nameof(loanRepository));
            _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
            _readerService = readerService ?? throw new ArgumentNullException(nameof(readerService));
            _loanService = loanService ?? throw new ArgumentNullException(nameof(loanService));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }


        public async Task<CreateLoanCommandOutput> Handle(CreateLoanCommandInput input, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(input, ct);
            var created = await _loanService.CreateLoan(input.ToData(), ct);
            return created.ToCreateLoanOutput();
        }

    }
}

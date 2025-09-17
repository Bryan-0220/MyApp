using Application.Interfaces;
using FluentValidation;
using Application.Loans.Services;
using Application.Loans.Mappers;

namespace DeleteLoan
{
    public class DeleteLoanCommandHandler : IDeleteLoanCommandHandler
    {
        private const string LoanNotFoundMessage = "Loan not found.";
        private const string CannotDeleteActiveMessage = "Cannot delete an active or overdue loan. Mark it as returned before deletion.";
        private const string LoanDeletedMessage = "Loan deleted.";

        private readonly ILoanRepository _loanRepository;
        private readonly IValidator<DeleteLoanCommandInput> _validator;
        private readonly ILoanService _loanService;

        public DeleteLoanCommandHandler(
            ILoanRepository loanRepository,
            IValidator<DeleteLoanCommandInput> validator,
            ILoanService loanService)
        {
            _loanRepository = loanRepository ?? throw new ArgumentNullException(nameof(loanRepository));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _loanService = loanService ?? throw new ArgumentNullException(nameof(loanService));
        }

        public async Task<DeleteLoanCommandOutput> Handle(DeleteLoanCommandInput input, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(input, ct);
            var result = await _loanService.DeleteLoan(input.Id, ct);
            return result.ToDeleteLoanOutput();
        }

    }
}

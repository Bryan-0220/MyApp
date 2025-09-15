using Application.Interfaces;
using FluentValidation;
using Application.Loans.Services;

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

        public async Task<DeleteLoanCommandOutput> HandleAsync(DeleteLoanCommandInput input, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(input, ct);

            var existing = await _loanRepository.GetByIdAsync(input.Id, ct);
            if (existing is null)
            {
                return Failure(LoanNotFoundMessage);
            }

            if (!await _loanService.EnsureCanDeleteAsync(existing, ct))
            {
                return Failure(CannotDeleteActiveMessage);
            }

            try
            {
                await _loanService.HandlePostDeleteAsync(existing, ct);
            }
            catch (Exception ex)
            {
                return Failure(ex.Message);
            }

            await _loanRepository.DeleteAsync(input.Id, ct);
            return Success();
        }

        private static DeleteLoanCommandOutput Failure(string message)
        {
            return new DeleteLoanCommandOutput { Deleted = false, Message = message };
        }

        private static DeleteLoanCommandOutput Success()
        {
            return new DeleteLoanCommandOutput { Deleted = true, Message = LoanDeletedMessage };
        }
    }
}

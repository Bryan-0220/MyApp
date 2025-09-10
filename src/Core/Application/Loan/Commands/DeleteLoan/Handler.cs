using Application.Interfaces;
using FluentValidation;

namespace DeleteLoan
{
    public class DeleteLoanCommandHandler : IDeleteLoanCommandHandler
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IValidator<DeleteLoanCommandInput> _validator;

        public DeleteLoanCommandHandler(ILoanRepository loanRepository, IValidator<DeleteLoanCommandInput> validator)
        {
            _loanRepository = loanRepository;
            _validator = validator;
        }

        public async Task<DeleteLoanCommandOutput> HandleAsync(DeleteLoanCommandInput input, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(input, ct);

            var existing = await _loanRepository.GetByIdAsync(input.Id, ct);
            if (existing is null)
            {
                return new DeleteLoanCommandOutput
                {
                    Deleted = false,
                    Message = "Loan not found"
                };
            }

            await _loanRepository.DeleteAsync(input.Id, ct);
            return new DeleteLoanCommandOutput
            {
                Deleted = true,
                Message = "Loan deleted"
            };
        }
    }
}

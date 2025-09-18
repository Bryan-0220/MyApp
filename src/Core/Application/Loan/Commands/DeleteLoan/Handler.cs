using Application.Interfaces;
using FluentValidation;
using Application.Loans.Services;
using Application.Loans.Mappers;

namespace DeleteLoan
{
    public class DeleteLoanCommandHandler : IDeleteLoanCommandHandler
    {
        private readonly IValidator<DeleteLoanCommandInput> _validator;
        private readonly ILoanService _loanService;

        public DeleteLoanCommandHandler(
            IValidator<DeleteLoanCommandInput> validator,
            ILoanService loanService)
        {
            _validator = validator;
            _loanService = loanService;
        }

        public async Task<DeleteLoanCommandOutput> Handle(DeleteLoanCommandInput input, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(input, ct);
            var result = await _loanService.DeleteLoan(input.Id, ct);
            return result.ToDeleteLoanOutput();
        }

    }
}

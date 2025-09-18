using Application.Loans.Mappers;
using Application.Loans.Services;
using FluentValidation;

namespace UpdateLoan
{
    public class UpdateLoanCommandHandler : IUpdateLoanCommandHandler
    {
        private readonly Application.Loans.Services.ILoanService _loanService;
        private readonly IValidator<UpdateLoanCommandInput> _validator;

        public UpdateLoanCommandHandler(Application.Loans.Services.ILoanService loanService, IValidator<UpdateLoanCommandInput> validator)
        {
            _loanService = loanService ?? throw new ArgumentNullException(nameof(loanService));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public async Task<UpdateLoanCommandOutput?> Handle(UpdateLoanCommandInput input, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(input, ct);

            var updated = await _loanService.UpdateLoan(input, ct);
            return updated.ToUpdateLoanOutput();
        }
    }
}

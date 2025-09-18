using Application.Loans.Mappers;
using Application.Loans.Services;
using FluentValidation;

namespace CreateLoan
{
    public class CreateLoanCommandHandler : ICreateLoanCommandHandler
    {
        private readonly ILoanService _loanService;
        private readonly IValidator<CreateLoanCommandInput> _validator;

        public CreateLoanCommandHandler(
            ILoanService loanService,
            IValidator<CreateLoanCommandInput> validator)
        {
            _loanService = loanService;
            _validator = validator;
        }

        public async Task<CreateLoanCommandOutput> Handle(CreateLoanCommandInput input, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(input, ct);
            var created = await _loanService.CreateLoan(input.ToData(), ct);
            return created.ToCreateLoanOutput();
        }

    }
}

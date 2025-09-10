using FluentValidation;

namespace GetLoanById
{
    public class GetLoanByIdQueryValidator : AbstractValidator<GetLoanByIdQueryInput>
    {
        public GetLoanByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required")
                .Matches("^[{(]?[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}[)}]?$")
                .WithMessage("Id must be a valid GUID");
        }
    }
}

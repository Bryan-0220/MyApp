using FluentValidation;

namespace FilterReaders
{
    public class FilterReadersQueryValidator : AbstractValidator<FilterReadersQueryInput>
    {
        public FilterReadersQueryValidator()
        {
            RuleFor(x => x.FirstName).Must(n => string.IsNullOrWhiteSpace(n) == false).When(x => x.FirstName != null)
                .WithMessage("FirstName must not be empty if provided.");

            RuleFor(x => x.LastName).Must(n => string.IsNullOrWhiteSpace(n) == false).When(x => x.LastName != null)
                .WithMessage("LastName must not be empty if provided.");

            RuleFor(x => x.Email).EmailAddress().When(x => x.Email != null)
                .WithMessage("Email must be valid if provided.");
        }
    }
}

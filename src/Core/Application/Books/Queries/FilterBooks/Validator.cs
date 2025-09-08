using FluentValidation;

namespace FilterBooks
{
    public class FilterBooksQueryValidator : AbstractValidator<FilterBooksQueryInput>
    {
        public FilterBooksQueryValidator()
        {
            RuleFor(x => x.Genre).Must(g => string.IsNullOrWhiteSpace(g) == false).When(x => x.Genre != null)
                .WithMessage("Genre must not be empty if provided.");
        }
    }
}

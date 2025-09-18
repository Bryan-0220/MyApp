using FluentValidation;

namespace FilterLoans
{
    public class FilterLoansQueryValidator : AbstractValidator<FilterLoansQueryInput>
    {
        public FilterLoansQueryValidator()
        {
            RuleFor(x => x.BookId)
                .Must(s => string.IsNullOrWhiteSpace(s) || s != "string");

            RuleFor(x => x.ReaderId)
                .Must(s => string.IsNullOrWhiteSpace(s) || s != "string");

            RuleFor(x => x.LoanDate)
                .NotNull().WithMessage("LoanDate is required");

            RuleFor(x => x.DueDate)
                .NotNull().WithMessage("DueDate is required");

            RuleFor(x => x)
                .Must(f => !(f.LoanDate.HasValue && f.DueDate.HasValue) || f.LoanDate <= f.DueDate)
                .WithMessage("DueDate must be later or equal to LoanDate");
        }
    }
}

using Application.Interfaces;
using Application.Filters;
using FluentValidation;
using Application.Loans.Mappers;

namespace FilterLoans
{
    public class FilterLoansQueryHandler : IFilterLoansQueryHandler
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IValidator<FilterLoansQueryInput> _validator;

        public FilterLoansQueryHandler(ILoanRepository loanRepository, IValidator<FilterLoansQueryInput> validator)
        {
            _loanRepository = loanRepository;
            _validator = validator;
        }

        public async Task<IEnumerable<FilterLoansQueryOutput>> Handle(FilterLoansQueryInput input, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(input, ct);
            var filter = LoanFilter.Create(loanDate: input.LoanDate, dueDate: input.DueDate);
            var loans = await _loanRepository.Filter(filter, ct);
            return loans.Select(loan => loan.ToFilterLoansOutput());
        }
    }
}

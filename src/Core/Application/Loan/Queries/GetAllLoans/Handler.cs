using Application.Interfaces;
using Application.Loans.Mappers;

namespace GetAllLoans
{
    public class GetAllLoansQueryHandler : IGetAllLoansQueryHandler
    {
        private readonly ILoanRepository _loanRepository;

        public GetAllLoansQueryHandler(ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
        }

        public async Task<IEnumerable<GetAllLoansQueryOutput>> HandleAsync(GetAllLoansQueryInput query, CancellationToken ct = default)
        {
            var loans = await _loanRepository.GetAllAsync(ct);

            return loans.Select(b => b.ToGetAllLoansOutput());
        }
    }
}

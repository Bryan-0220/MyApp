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

        public async Task<IEnumerable<GetAllLoansQueryOutput>> Handle(GetAllLoansQueryInput query, CancellationToken ct = default)
        {
            var loans = await _loanRepository.GetAll(ct);
            return loans.Select(loan => loan.ToGetAllLoansOutput());
        }
    }
}

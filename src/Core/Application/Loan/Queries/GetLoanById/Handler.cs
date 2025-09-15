using Application.Interfaces;
using Application.Loans.Mappers;

namespace GetLoanById
{
    public class GetLoanByIdQueryHandler : IGetLoanByIdQueryHandler
    {
        private readonly ILoanRepository _loanRepository;

        public GetLoanByIdQueryHandler(ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
        }

        public async Task<GetLoanByIdQueryOutput?> Handle(GetLoanByIdQueryInput query, CancellationToken ct = default)
        {
            var loan = await _loanRepository.GetById(query.Id, ct);
            if (loan is null) return null;

            return loan.ToGetLoanByIdOutput();
        }
    }
}

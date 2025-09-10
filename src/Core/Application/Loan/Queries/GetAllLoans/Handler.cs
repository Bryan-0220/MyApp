using Application.Interfaces;

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

            return loans.Select(b => new GetAllLoansQueryOutput
            {
                Id = b.Id,
                BookId = b.BookId,
                ReaderId = b.ReaderId,
                LoanDate = b.LoanDate,
                DueDate = b.DueDate,
                ReturnedDate = b.ReturnedDate,
                Status = b.Status.ToString()
            });
        }
    }
}

using Application.Interfaces;

namespace GetLoanById
{
    public class GetLoanByIdQueryHandler : IGetLoanByIdQueryHandler
    {
        private readonly ILoanRepository _loanRepository;

        public GetLoanByIdQueryHandler(ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
        }

        public async Task<GetLoanByIdQueryOutput?> HandleAsync(GetLoanByIdQueryInput query, CancellationToken ct = default)
        {
            var loan = await _loanRepository.GetByIdAsync(query.Id, ct);
            if (loan is null) return null;

            return new GetLoanByIdQueryOutput
            {
                Id = loan.Id,
                BookId = loan.BookId,
                ReaderId = loan.ReaderId,
                LoanDate = loan.LoanDate,
                DueDate = loan.DueDate,
                ReturnedDate = loan.ReturnedDate,
                Status = loan.Status.ToString()
            };
        }
    }
}

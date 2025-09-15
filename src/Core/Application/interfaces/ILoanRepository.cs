using Domain.Models;
using Application.Filters;

namespace Application.Interfaces
{
    public interface ILoanRepository : IRepository<Loan>
    {
        Task MarkReturned(string loanId, DateOnly returnedDate, CancellationToken ct = default);

        Task<IEnumerable<Loan>> Filter(LoanFilter? filter = null, CancellationToken ct = default);
    }
}

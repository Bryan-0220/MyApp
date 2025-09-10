using Domain.Models;
using Application.Filters;

namespace Application.Interfaces
{
    public interface ILoanRepository : IRepository<Loan>
    {
        Task MarkReturnedAsync(string loanId, DateOnly returnedDate, CancellationToken ct = default);

        Task<IEnumerable<Loan>> FilterAsync(LoanFilter? filter = null, CancellationToken ct = default);
    }
}

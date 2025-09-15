using Domain.Models;

namespace Application.Loans.Services
{
    public interface ILoanService
    {
        Task EnsureNoDuplicateLoan(string bookId, string readerId, CancellationToken ct = default);
        Task<bool> EnsureCanDelete(Loan loan, CancellationToken ct = default);
        Task HandlePostDelete(Loan loan, CancellationToken ct = default);
    }
}

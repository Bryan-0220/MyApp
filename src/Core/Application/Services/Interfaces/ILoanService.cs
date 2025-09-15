using Domain.Models;

namespace Application.Loans.Services
{
    public interface ILoanService
    {
        Task EnsureNoDuplicateLoanAsync(string bookId, string readerId, CancellationToken ct = default);
        Task<bool> EnsureCanDeleteAsync(Loan loan, CancellationToken ct = default);
        Task HandlePostDeleteAsync(Loan loan, CancellationToken ct = default);
    }
}

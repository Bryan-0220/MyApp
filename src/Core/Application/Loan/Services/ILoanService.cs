namespace Application.Loans.Services
{
    public interface ILoanService
    {
        Task EnsureNoDuplicateLoanAsync(string bookId, string readerId, CancellationToken ct = default);
    }
}

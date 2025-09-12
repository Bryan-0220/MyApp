using System.Linq.Expressions;
using Domain.Models;
using Application.Filters;
using Application.Interfaces;
using MongoDB.Driver;

namespace Infrastructure.Repositories
{
    public class LoanRepository : ILoanRepository
    {
        private readonly IMongoCollection<Loan> _loans;

        public LoanRepository(IMongoDatabase database)
        {
            _loans = database.GetCollection<Loan>("loans");
        }

        public async Task<Loan> CreateAsync(Loan entity, CancellationToken ct = default)
        {
            await _loans.InsertOneAsync(entity, cancellationToken: ct);
            return entity;
        }

        public async Task<bool> DeleteAsync(string id, CancellationToken ct = default)
        {
            var res = await _loans.DeleteOneAsync(b => b.Id == id, ct);
            return res.DeletedCount > 0;
        }

        public async Task<IEnumerable<Loan>> FilterAsync(LoanFilter? filter = null, CancellationToken ct = default)
        {
            var builder = Builders<Loan>.Filter;
            var f = builder.Empty;

            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.BookId))
                    f = f & builder.Eq(b => b.BookId, filter.BookId);
                if (!string.IsNullOrWhiteSpace(filter.UserId))
                    f = f & builder.Eq(b => b.ReaderId, filter.UserId);
                if (filter.Returned.HasValue)
                {
                    if (filter.Returned.Value)
                        f = f & builder.Ne(b => b.ReturnedDate, null);
                    else
                        f = f & builder.Eq(b => b.ReturnedDate, null);
                }
                if (filter.LoanDate.HasValue)
                    f = f & builder.Eq(b => b.LoanDate, filter.LoanDate.Value);
                if (filter.DueDate.HasValue)
                    f = f & builder.Eq(b => b.DueDate, filter.DueDate.Value);
                if (filter.ReturnedDate.HasValue)
                    f = f & builder.Eq(b => b.ReturnedDate, filter.ReturnedDate.Value);
            }

            var cursor = await _loans.FindAsync(f, cancellationToken: ct);
            return await cursor.ToListAsync(ct);
        }

        public async Task<IEnumerable<Loan>> GetAllAsync(CancellationToken ct = default)
        {
            var cursor = await _loans.FindAsync(Builders<Loan>.Filter.Empty, cancellationToken: ct);
            return await cursor.ToListAsync(ct);
        }

        public async Task<Loan?> GetByIdAsync(string id, CancellationToken ct = default)
        {
            var cursor = await _loans.FindAsync(b => b.Id == id, cancellationToken: ct);
            return await cursor.FirstOrDefaultAsync(ct);
        }

        public async Task<long> CountAsync(Expression<Func<Loan, bool>>? predicate = null, CancellationToken ct = default)
        {
            if (predicate == null) return await _loans.CountDocumentsAsync(Builders<Loan>.Filter.Empty, cancellationToken: ct);
            return await _loans.CountDocumentsAsync(Builders<Loan>.Filter.Where(predicate), cancellationToken: ct);
        }

        public async Task<IEnumerable<Loan>> ListAsync(CancellationToken ct = default)
        {
            return await GetAllAsync(ct);
        }

        public async Task<bool> UpdateAsync(string id, Loan entity, CancellationToken ct = default)
        {
            var res = await _loans.ReplaceOneAsync(b => b.Id == id, entity, cancellationToken: ct);
            return res.ModifiedCount > 0 || res.MatchedCount > 0;
        }

        public async Task UpdateAsync(Loan entity, CancellationToken ct = default)
        {
            await UpdateAsync(entity.Id, entity, ct);
        }

        public async Task MarkReturnedAsync(string loanId, DateOnly returnedDate, CancellationToken ct = default)
        {
            var update = Builders<Loan>.Update.Set(l => l.ReturnedDate, returnedDate).Set(l => l.Status, LoanStatus.Returned);
            var res = await _loans.UpdateOneAsync(l => l.Id == loanId, update, cancellationToken: ct);
        }
    }
}

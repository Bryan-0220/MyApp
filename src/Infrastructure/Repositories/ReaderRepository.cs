using System.Linq.Expressions;
using Domain.Models;
using Application.Filters;
using Application.Interfaces;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Infrastructure.Repositories
{
    public class ReaderRepository : IReaderRepository
    {
        private readonly IMongoCollection<Reader> _readers;

        public ReaderRepository(IMongoDatabase database)
        {
            _readers = database.GetCollection<Reader>("readers");
        }

        public async Task<Reader> CreateAsync(Reader entity, CancellationToken ct = default)
        {
            await _readers.InsertOneAsync(entity, cancellationToken: ct);
            return entity;
        }

        public async Task<bool> DeleteAsync(string id, CancellationToken ct = default)
        {
            var res = await _readers.DeleteOneAsync(r => r.Id == id, ct);
            return res.DeletedCount > 0;
        }

        public async Task<IEnumerable<Reader>> FilterAsync(ReaderFilter? filter = null, CancellationToken ct = default)
        {
            var builder = Builders<Reader>.Filter;
            var f = builder.Empty;

            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.FirstName))
                    f = f & builder.Regex(r => r.FirstName, new BsonRegularExpression(filter.FirstName, "i"));

                if (!string.IsNullOrWhiteSpace(filter.LastName))
                    f = f & builder.Regex(r => r.LastName, new BsonRegularExpression(filter.LastName, "i"));
            }

            var cursor = await _readers.FindAsync(f, cancellationToken: ct);
            return await cursor.ToListAsync(ct);
        }

        public async Task<IEnumerable<Reader>> GetAllAsync(CancellationToken ct = default)
        {
            var cursor = await _readers.FindAsync(Builders<Reader>.Filter.Empty, cancellationToken: ct);
            return await cursor.ToListAsync(ct);
        }

        public async Task<Reader?> GetByIdAsync(string id, CancellationToken ct = default)
        {
            var cursor = await _readers.FindAsync(r => r.Id == id, cancellationToken: ct);
            return await cursor.FirstOrDefaultAsync(ct);
        }

        public async Task<long> CountAsync(Expression<Func<Reader, bool>>? predicate = null, CancellationToken ct = default)
        {
            if (predicate == null) return await _readers.CountDocumentsAsync(Builders<Reader>.Filter.Empty, cancellationToken: ct);
            return await _readers.CountDocumentsAsync(Builders<Reader>.Filter.Where(predicate), cancellationToken: ct);
        }

        public async Task<IEnumerable<Reader>> ListAsync(CancellationToken ct = default)
        {
            return await GetAllAsync(ct);
        }

        public async Task<bool> UpdateAsync(string id, Reader entity, CancellationToken ct = default)
        {
            try
            {
                var res = await _readers.ReplaceOneAsync(r => r.Id == id, entity, cancellationToken: ct);
                return res.ModifiedCount > 0 || res.MatchedCount > 0;
            }
            catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
            {
                throw new InvalidOperationException("Duplicate key in readers collection.");
            }
        }

        public async Task UpdateAsync(Reader entity, CancellationToken ct = default)
        {
            await UpdateAsync(entity.Id, entity, ct);
        }
    }
}

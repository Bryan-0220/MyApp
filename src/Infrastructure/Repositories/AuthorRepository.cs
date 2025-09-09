using System.Linq.Expressions;
using Domain.Models;
using Application.Filters;
using Application.Interfaces;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Infrastructure.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly IMongoCollection<Author> _authors;

        public AuthorRepository(IMongoDatabase database)
        {
            _authors = database.GetCollection<Author>("authors");
        }

        public async Task<Author> CreateAsync(Author entity, CancellationToken ct = default)
        {
            await _authors.InsertOneAsync(entity, cancellationToken: ct);
            return entity;
        }

        public async Task<bool> DeleteAsync(string id, CancellationToken ct = default)
        {
            var res = await _authors.DeleteOneAsync(a => a.Id == id, ct);
            return res.DeletedCount > 0;
        }

        public async Task<IEnumerable<Author>> FilterAsync(Expression<Func<Author, bool>>? predicate = null, CancellationToken ct = default)
        {
            if (predicate == null) return await (await _authors.FindAsync(Builders<Author>.Filter.Empty, cancellationToken: ct)).ToListAsync(ct);
            var cursor = await _authors.FindAsync(predicate, cancellationToken: ct);
            return await cursor.ToListAsync(ct);
        }

        public async Task<IEnumerable<Author>> FilterAsync(AuthorFilter? filter = null, CancellationToken ct = default)
        {
            var builder = Builders<Author>.Filter;
            var f = builder.Empty;

            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.Name))
                    f = f & builder.Regex(a => a.Name, new BsonRegularExpression(filter.Name, "i"));
                if (!string.IsNullOrWhiteSpace(filter.Id))
                    f = f & builder.Eq(a => a.Id, filter.Id);
                if (filter.BirthYear.HasValue)
                    f = f & builder.Eq(a => a.BirthDate.HasValue ? a.BirthDate.Value.Year : -1, filter.BirthYear.Value);
                if (filter.Genres != null && System.Linq.Enumerable.Any(filter.Genres))
                    f = f & builder.All(a => a.Genres, filter.Genres);
            }

            var cursor = await _authors.FindAsync(f, cancellationToken: ct);
            return await cursor.ToListAsync(ct);
        }

        public async Task<IEnumerable<Author>> GetAllAsync(CancellationToken ct = default)
        {
            var cursor = await _authors.FindAsync(Builders<Author>.Filter.Empty, cancellationToken: ct);
            return await cursor.ToListAsync(ct);
        }

        public async Task<Author?> GetByIdAsync(string id, CancellationToken ct = default)
        {
            var cursor = await _authors.FindAsync(a => a.Id == id, cancellationToken: ct);
            return await cursor.FirstOrDefaultAsync(ct);
        }

        public async Task<long> CountAsync(Expression<Func<Author, bool>>? predicate = null, CancellationToken ct = default)
        {
            if (predicate == null) return await _authors.CountDocumentsAsync(Builders<Author>.Filter.Empty, cancellationToken: ct);
            return await _authors.CountDocumentsAsync(Builders<Author>.Filter.Where(predicate), cancellationToken: ct);
        }

        public async Task<IEnumerable<Author>> ListAsync(CancellationToken ct = default)
        {
            return await GetAllAsync(ct);
        }

        public async Task<bool> UpdateAsync(string id, Author entity, CancellationToken ct = default)
        {
            try
            {
                var res = await _authors.ReplaceOneAsync(a => a.Id == id, entity, cancellationToken: ct);
                return res.ModifiedCount > 0 || res.MatchedCount > 0;
            }
            catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
            {
                throw new InvalidOperationException("Duplicate key error when updating author.");
            }
        }

        public async Task UpdateAsync(Author entity, CancellationToken ct = default)
        {
            await UpdateAsync(entity.Id, entity, ct);
        }
    }
}

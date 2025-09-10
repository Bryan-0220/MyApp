using System.Linq.Expressions;
using Domain.Models;
using Application.Filters;
using Application.Interfaces;
using MongoDB.Driver;

namespace Infrastructure.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly IMongoCollection<Book> _books;

        public BookRepository(IMongoDatabase database)
        {
            _books = database.GetCollection<Book>("books");
        }

        public async Task<Book> CreateAsync(Book entity, CancellationToken ct = default)
        {
            await _books.InsertOneAsync(entity, cancellationToken: ct);
            return entity;
        }

        public async Task<bool> DeleteAsync(string id, CancellationToken ct = default)
        {
            var res = await _books.DeleteOneAsync(b => b.Id == id, ct);
            return res.DeletedCount > 0;
        }

        public async Task<IEnumerable<Book>> FilterAsync(BookFilter? filter = null, CancellationToken ct = default)
        {
            var builder = Builders<Book>.Filter;
            var f = builder.Empty;

            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.Title))
                    f = f & builder.Regex(b => b.Title, new MongoDB.Bson.BsonRegularExpression(filter.Title, "i"));
                if (!string.IsNullOrWhiteSpace(filter.AuthorId))
                    f = f & builder.Eq(b => b.AuthorId, filter.AuthorId);
                if (!string.IsNullOrWhiteSpace(filter.Isbn))
                    f = f & builder.Eq(b => b.ISBN, filter.Isbn);
                if (filter.PublishedYear.HasValue)
                    f = f & builder.Eq(b => b.PublishedYear, filter.PublishedYear.Value);

                if (filter.Available.HasValue)
                {
                    if (filter.Available.Value)
                        f = f & builder.Gt(b => b.CopiesAvailable, 0);
                    else
                        f = f & builder.Lte(b => b.CopiesAvailable, 0);
                }
            }

            var cursor = await _books.FindAsync(f, cancellationToken: ct);
            return await cursor.ToListAsync(ct);
        }

        public async Task<IEnumerable<Book>> FilterAsync(Expression<Func<Book, bool>>? predicate = null, CancellationToken ct = default)
        {
            if (predicate == null) return await (await _books.FindAsync(Builders<Book>.Filter.Empty, cancellationToken: ct)).ToListAsync(ct);
            var cursor = await _books.FindAsync(predicate, cancellationToken: ct);
            return await cursor.ToListAsync(ct);
        }

        public async Task<IEnumerable<Book>> GetAllAsync(CancellationToken ct = default)
        {
            var cursor = await _books.FindAsync(Builders<Book>.Filter.Empty, cancellationToken: ct);
            return await cursor.ToListAsync(ct);
        }

        public async Task<Book?> GetByIdAsync(string id, CancellationToken ct = default)
        {
            var cursor = await _books.FindAsync(b => b.Id == id, cancellationToken: ct);
            return await cursor.FirstOrDefaultAsync(ct);
        }

        public async Task<long> CountAsync(Expression<Func<Book, bool>>? predicate = null, CancellationToken ct = default)
        {
            if (predicate == null) return await _books.CountDocumentsAsync(Builders<Book>.Filter.Empty, cancellationToken: ct);
            return await _books.CountDocumentsAsync(Builders<Book>.Filter.Where(predicate), cancellationToken: ct);
        }

        public async Task<IEnumerable<Book>> ListAsync(CancellationToken ct = default)
        {
            return await GetAllAsync(ct);
        }

        public async Task<bool> TryChangeCopiesAsync(string bookId, int numberOfCopiesAfected, CancellationToken ct = default)
        {
            var update = Builders<Book>.Update.Inc(b => b.CopiesAvailable, numberOfCopiesAfected);
            var options = new FindOneAndUpdateOptions<Book>
            {
                ReturnDocument = ReturnDocument.After
            };

            var filter = Builders<Book>.Filter.And(
                Builders<Book>.Filter.Eq(b => b.Id, bookId),
                Builders<Book>.Filter.Gte(b => b.CopiesAvailable, Math.Max(0, -numberOfCopiesAfected))
            );

            var updated = await _books.FindOneAndUpdateAsync(filter, update, options, ct);
            return updated != null;
        }

        public async Task<bool> UpdateAsync(string id, Book entity, CancellationToken ct = default)
        {
            try
            {
                var res = await _books.ReplaceOneAsync(b => b.Id == id, entity, cancellationToken: ct);
                return res.ModifiedCount > 0 || res.MatchedCount > 0;
            }
            catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
            {
                throw new InvalidOperationException("Ya existe un libro con ese ISBN.");
            }
        }

        public async Task UpdateAsync(Book entity, CancellationToken ct = default)
        {
            await UpdateAsync(entity.Id, entity, ct);
        }
    }
}

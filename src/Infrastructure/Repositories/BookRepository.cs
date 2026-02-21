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

        public async Task<Book> Create(Book entity, CancellationToken ct = default)
        {
            await _books.InsertOneAsync(entity, cancellationToken: ct);
            return entity;
        }

        public async Task<bool> Delete(string id, CancellationToken ct = default)
        {
            var res = await _books.DeleteOneAsync(b => b.Id == id, ct);
            return res.DeletedCount > 0;
        }

        public async Task<IEnumerable<Book>> Filter(BookFilter? filter = null, CancellationToken ct = default)
        {
            var builder = Builders<Book>.Filter;
            var f = builder.Empty;

            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.Title))
                {
                    // case-insensitive contains match for title
                    f = f & builder.Regex(b => b.Title, new MongoDB.Bson.BsonRegularExpression(filter.Title, "i"));
                }

                if (!string.IsNullOrWhiteSpace(filter.AuthorId))
                {
                    f = f & builder.Eq(b => b.AuthorId, filter.AuthorId);
                }

                if (!string.IsNullOrWhiteSpace(filter.Isbn))
                {
                    // ISBN stored in Book.ISBN; use equality match (normalized elsewhere)
                    f = f & builder.Eq(b => b.ISBN, filter.Isbn);
                }

                if (filter.PublishedYear.HasValue)
                {
                    f = f & builder.Eq(b => b.PublishedYear, filter.PublishedYear.Value);
                }

                if (filter.Available.HasValue)
                {
                    if (filter.Available.Value)
                        f = f & builder.Gt(b => b.CopiesAvailable, 0);
                    else
                        f = f & builder.Eq(b => b.CopiesAvailable, 0);
                }

                if (!string.IsNullOrWhiteSpace(filter.Genre))
                {
                    f = f & builder.Regex(b => b.Genre, new MongoDB.Bson.BsonRegularExpression(filter.Genre, "i"));
                }
            }

            var cursor = await _books.FindAsync(f, cancellationToken: ct);
            return await cursor.ToListAsync(ct);
        }

        public async Task<IEnumerable<Book>> GetAll(CancellationToken ct = default)
        {
            var cursor = await _books.FindAsync(Builders<Book>.Filter.Empty, cancellationToken: ct);
            return await cursor.ToListAsync(ct);
        }

        public async Task<Book?> GetById(string id, CancellationToken ct = default)
        {
            var cursor = await _books.FindAsync(b => b.Id == id, cancellationToken: ct);
            return await cursor.FirstOrDefaultAsync(ct);
        }

        public async Task<long> Count(Expression<Func<Book, bool>>? predicate = null, CancellationToken ct = default)
        {
            if (predicate == null) return await _books.CountDocumentsAsync(Builders<Book>.Filter.Empty, cancellationToken: ct);
            return await _books.CountDocumentsAsync(Builders<Book>.Filter.Where(predicate), cancellationToken: ct);
        }

        public async Task<IEnumerable<Book>> List(CancellationToken ct = default)
        {
            return await GetAll(ct);
        }

        public async Task<bool> TryChangeCopies(string bookId, int numberOfCopiesAfected, CancellationToken ct = default)
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

        public async Task<bool> Update(Book book, CancellationToken ct = default)
        {
            try
            {
                var res = await _books.ReplaceOneAsync(b => b.Id == book.Id, book, cancellationToken: ct);
                return res.ModifiedCount > 0 || res.MatchedCount > 0;
            }
            catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
            {
                throw new InvalidOperationException("Ya existe un libro con ese ISBN.");
            }
        }

    }
}

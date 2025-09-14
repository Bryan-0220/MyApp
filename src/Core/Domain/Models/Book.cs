using MongoDB.Bson.Serialization.Attributes;
using Domain.Common;

namespace Domain.Models
{
    public class Book
    {
        [BsonId]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; private set; } = string.Empty;
        public string AuthorId { get; private set; } = string.Empty;
        public string? ISBN { get; private set; }
        public int? PublishedYear { get; private set; }
        public int CopiesAvailable { get; private set; } = 1;
        public string Genre { get; private set; } = string.Empty;

        public Book() { }

        public static Book Create(string title, string authorId, string? isbn, int? publishedYear, int copiesAvailable, string genre)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new DomainException("Title is required");
            if (copiesAvailable < 0) throw new DomainException("CopiesAvailable must be >= 0");
            if (string.IsNullOrWhiteSpace(genre)) throw new DomainException("Genre is required");

            var book = new Book
            {
                Title = title.Trim(),
                AuthorId = (authorId ?? string.Empty).Trim(),
                PublishedYear = publishedYear,
                CopiesAvailable = copiesAvailable,
                Genre = genre.Trim()
            };

            if (!string.IsNullOrWhiteSpace(isbn))
            {
                if (!Domain.ValueObjects.Isbn.TryParse(isbn, out var vo, out var error))
                    throw new DomainException(error ?? "Invalid ISBN");

                book.ISBN = vo!.Value;
            }

            return book;
        }

        public void SetTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new DomainException("Title is required");
            Title = title.Trim();
        }

        public void SetAuthor(string authorId)
        {
            AuthorId = (authorId ?? string.Empty).Trim();
        }

        public void SetIsbn(string? isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn))
            {
                ISBN = null;
                return;
            }

            if (!Domain.ValueObjects.Isbn.TryParse(isbn, out var vo, out var error))
                throw new DomainException(error ?? "Invalid ISBN");

            ISBN = vo!.Value;
        }

        public void SetPublishedYear(int? year)
        {
            PublishedYear = year;
        }

        public void SetCopiesAvailable(int copies)
        {
            if (copies < 0) throw new DomainException("CopiesAvailable must be >= 0");
            CopiesAvailable = copies;
        }

        public void EnsureHasAvailableCopies()
        {
            if (CopiesAvailable < 1) throw new DomainException("No copies available for this book");
        }

        public void SetGenre(string genre)
        {
            if (string.IsNullOrWhiteSpace(genre)) throw new DomainException("Genre is required");
            Genre = genre.Trim();
        }
    }
}

using MongoDB.Bson.Serialization.Attributes;
using Domain.Common;
using Domain.ValueObjects;

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

        public static Book Create(BookData data)
        {
            ValidateForCreate(data);

            var book = new Book();

            book.SetTitle(data.Title!);
            book.SetAuthor(data.AuthorId!);
            book.SetPublishedYear(data.PublishedYear);
            book.SetCopiesAvailable(data.CopiesAvailable);
            book.SetGenre(data.Genre!);
            book.SetIsbn(data.ISBN);

            return book;
        }

        private static void ValidateForCreate(BookData? data)
        {
            if (data == null) throw new BusinessRuleException("Input is required");
            if (string.IsNullOrWhiteSpace(data.Title)) throw new BusinessRuleException("Title is required");
            if (data.CopiesAvailable < 0) throw new BusinessRuleException("CopiesAvailable must be >= 0");
            if (string.IsNullOrWhiteSpace(data.AuthorId)) throw new BusinessRuleException("AuthorId is required");
            if (string.IsNullOrWhiteSpace(data.Genre)) throw new BusinessRuleException("Genre is required");
        }

        public void SetTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new BusinessRuleException("Title is required");
            Title = StringNormalizer.Normalize(title) ?? string.Empty;
        }

        public void SetAuthor(string authorId)
        {
            if (string.IsNullOrWhiteSpace(authorId)) throw new BusinessRuleException("AuthorId is required");
            AuthorId = StringNormalizer.Normalize(authorId) ?? string.Empty;
        }

        public void SetIsbn(string? isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn))
            {
                ISBN = null;
                return;
            }

            if (!Isbn.TryParse(isbn, out var vo, out var error))
                throw new BusinessRuleException(error ?? "Invalid ISBN");

            ISBN = vo!.Value;
        }

        public void SetPublishedYear(int? year)
        {
            PublishedYear = year;
        }

        public void SetCopiesAvailable(int copies)
        {
            if (copies < 0) throw new BusinessRuleException("CopiesAvailable must be >= 0");
            CopiesAvailable = copies;
        }

        public void EnsureHasAvailableCopies()
        {
            if (CopiesAvailable < 1) throw new BusinessRuleException("No copies available for this book");
        }

        public void SetGenre(string genre)
        {
            if (string.IsNullOrWhiteSpace(genre)) throw new BusinessRuleException("Genre is required");
            Genre = StringNormalizer.Normalize(genre) ?? string.Empty;
        }
    }
}

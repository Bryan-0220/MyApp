namespace Application.Filters
{
    public class BookFilter
    {
        public string? Title { get; init; }
        public string? AuthorId { get; init; }
        public string? Isbn { get; init; }
        public int? PublishedYear { get; init; }
        public bool? Available { get; init; }
        public string? Genre { get; init; }

        public BookFilter() { }

        private BookFilter(string? title, string? authorId, string? isbn, int? publishedYear, bool? available, string? genre)
        {
            Title = title;
            AuthorId = authorId;
            Isbn = isbn;
            PublishedYear = publishedYear;
            Available = available;
            Genre = genre;
        }

        public static BookFilter Create(string? title = null, string? authorId = null, string? isbn = null, int? publishedYear = null, bool? available = null, string? genre = null)
        {
            var cleanedTitle = string.IsNullOrWhiteSpace(title) ? null : title.Trim();
            var cleanedAuthorId = string.IsNullOrWhiteSpace(authorId) ? null : authorId.Trim();
            var cleanedIsbn = string.IsNullOrWhiteSpace(isbn) ? null : isbn.Trim();
            var cleanedGenre = string.IsNullOrWhiteSpace(genre) ? null : genre.Trim();

            return new BookFilter(cleanedTitle, cleanedAuthorId, cleanedIsbn, publishedYear, available, cleanedGenre);
        }
    }
}

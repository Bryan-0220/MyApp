namespace Application.Filters
{
    public class BookFilter
    {
        public string? Title { get; set; }
        public string? AuthorId { get; set; }
        public string? Isbn { get; set; }
        public int? PublishedYear { get; set; }
        public bool? Available { get; set; }
        public string? Genre { get; set; }
    }
}

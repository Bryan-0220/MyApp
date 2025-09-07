namespace FilterBooks
{
    public class FilterBooksQueryOutput
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string AuthorId { get; set; } = string.Empty;
        public string? ISBN { get; set; }
        public int? PublishedYear { get; set; }
        public int CopiesAvailable { get; set; }
        public string Genre { get; set; } = string.Empty;
    }
}

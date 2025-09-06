namespace UpdateBook
{
    public class UpdateBookCommandInput
    {
        public string Id { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? AuthorId { get; set; }
        public string? ISBN { get; set; }
        public int? PublishedYear { get; set; }
        public int? CopiesAvailable { get; set; }
    }
}

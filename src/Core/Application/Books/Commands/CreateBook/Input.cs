namespace CreateBook
{
    public class CreateBookCommandInput
    {
        public string Title { get; set; } = string.Empty;
        public string AuthorId { get; set; } = string.Empty;
        public string? ISBN { get; set; }
        public int? PublishedYear { get; set; }
        public int CopiesAvailable { get; set; } = 1;
        public string Genre { get; set; } = string.Empty;
    }
}

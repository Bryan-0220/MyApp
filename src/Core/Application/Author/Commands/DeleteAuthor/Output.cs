namespace DeleteAuthor
{
    public class DeleteAuthorCommandOutput
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? AuthorId { get; set; }
    }
}

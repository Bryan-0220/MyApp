namespace DeleteBook
{
    public class DeleteBookCommandOutput
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? BookId { get; set; }
    }
}

namespace Domain.Models
{
    public class Book
    {
        public string Id { get; set; } = System.Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string AuthorId { get; set; } = string.Empty;
        public string? ISBN { get; set; }
        public int? PublishedYear { get; set; }
        public int CopiesAvailable { get; set; } = 1;
    }
}

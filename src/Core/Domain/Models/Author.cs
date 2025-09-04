namespace Domain.Models
{
    public class Author
    {
        public string Id { get; set; } = System.Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string? Bio { get; set; }
    }
}

namespace UpdateAuthor
{
    public class UpdateAuthorCommandOutput
    {
        public string Id { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? Bio { get; set; }
        public string? Nationality { get; set; }
        public DateOnly? BirthDate { get; set; }
        public DateOnly? DeathDate { get; set; }
        public IEnumerable<string> Genres { get; set; } = Array.Empty<string>();
    }
}

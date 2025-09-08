namespace CreateAuthor
{
    public class CreateAuthorCommandInput
    {
        public string Name { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? Nationality { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? DeathDate { get; set; }
        public IEnumerable<string>? Genres { get; set; }
    }
}

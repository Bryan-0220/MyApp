namespace CreateAuthor
{
    public class CreateAuthorCommandOutput
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? Nationality { get; set; }
        public System.DateTime? BirthDate { get; set; }
        public System.DateTime? DeathDate { get; set; }
        public System.Collections.Generic.IEnumerable<string> Genres { get; set; } = System.Array.Empty<string>();
    }
}

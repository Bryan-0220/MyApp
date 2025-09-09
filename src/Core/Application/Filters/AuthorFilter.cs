namespace Application.Filters
{
    public class AuthorFilter
    {
        public string? Name { get; set; }
        public string? Id { get; set; }
        public int? BirthYear { get; set; }
        public int? DeathYear { get; set; }
        public bool? Available { get; set; }
        public IEnumerable<string>? Genres { get; set; }
    }
}

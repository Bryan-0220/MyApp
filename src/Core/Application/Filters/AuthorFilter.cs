namespace Application.Filters
{
    public class AuthorFilter
    {
        public string? Name { get; init; }
        public string? Id { get; init; }
        public int? BirthYear { get; init; }
        public int? DeathYear { get; init; }
        public bool? Available { get; init; }
        public string[]? Genres { get; init; }

        public AuthorFilter() { }

        private AuthorFilter(string? name, string? id, int? birthYear, int? deathYear, bool? available, string[]? genres)
        {
            Name = name;
            Id = id;
            BirthYear = birthYear;
            DeathYear = deathYear;
            Available = available;
            Genres = genres;
        }

        public static AuthorFilter Create(string? name, IEnumerable<string>? genres, string? id = null, int? birthYear = null, int? deathYear = null, bool? available = null)
        {
            var cleanedName = string.IsNullOrWhiteSpace(name) ? null : name.Trim();

            var cleanedGenres = genres?
                .Where(g => !string.IsNullOrWhiteSpace(g))
                .Select(g => g.Trim())
                .ToArray();

            var finalGenres = cleanedGenres != null && cleanedGenres.Length > 0 ? cleanedGenres : null;

            return new AuthorFilter(cleanedName, id, birthYear, deathYear, available, finalGenres);
        }
    }
}

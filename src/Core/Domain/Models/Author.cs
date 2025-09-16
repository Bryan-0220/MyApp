using Domain.Common;

namespace Domain.Models
{
    public class Author
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; private set; } = string.Empty;
        public string? Bio { get; private set; }

        public string Nationality { get; private set; } = string.Empty;

        public DateOnly? BirthDate { get; private set; }
        public DateOnly? DeathDate { get; private set; }

        public HashSet<string> Genres { get; private set; } = new(StringComparer.OrdinalIgnoreCase);

        public Author() { }

        public static Author Create(AuthorData input)
        {
            if (input == null) throw new DomainException("Input is required");
            if (string.IsNullOrWhiteSpace(input.Name)) throw new DomainException("Name is required");
            if (input.BirthDate.HasValue && input.DeathDate.HasValue && input.DeathDate < input.BirthDate) throw new DomainException("DeathDate cannot be before BirthDate");

            if (input.Genres == null)
                throw new DomainException("Genres are required");

            // Ensure there's at least one non-empty genre before creating the object
            var hasAnyGenre = input.Genres.Any(g => !string.IsNullOrWhiteSpace(g));
            if (!hasAnyGenre)
                throw new DomainException("At least one genre is required");

            if (string.IsNullOrWhiteSpace(input.Nationality))
                throw new DomainException("Nationality is required");

            var a = new Author
            {
                Name = StringNormalizer.Normalize(input.Name) ?? string.Empty,
                Bio = string.IsNullOrWhiteSpace(input.Bio) ? null : StringNormalizer.Normalize(input.Bio),
                Nationality = StringNormalizer.Normalize(input.Nationality) ?? string.Empty,
                BirthDate = input.BirthDate,
                DeathDate = input.DeathDate
            };

            a.Genres = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var g in input.Genres)
            {
                if (string.IsNullOrWhiteSpace(g)) continue;
                a.Genres.Add(StringNormalizer.Normalize(g)!);
            }

            return a;
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Name is required");
            Name = StringNormalizer.Normalize(name) ?? string.Empty;
        }

        public void SetBio(string? bio)
        {
            Bio = string.IsNullOrWhiteSpace(bio) ? null : StringNormalizer.Normalize(bio);
        }

        public void SetNationality(string nationality)
        {
            Nationality = StringNormalizer.Normalize(nationality) ?? string.Empty;
        }

        public void SetBirthDate(DateOnly? birthDate)
        {
            BirthDate = birthDate;
            if (BirthDate.HasValue && DeathDate.HasValue && DeathDate < BirthDate) throw new DomainException("DeathDate cannot be before BirthDate");
        }

        public void SetDeathDate(DateOnly? deathDate)
        {
            DeathDate = deathDate;
            if (BirthDate.HasValue && DeathDate.HasValue && DeathDate < BirthDate) throw new DomainException("DeathDate cannot be before BirthDate");
        }

        public void AddGenre(string genre)
        {
            if (string.IsNullOrWhiteSpace(genre)) throw new DomainException("Genre is required");
            Genres.Add(StringNormalizer.Normalize(genre)!);
        }

        public void RemoveGenre(string genre)
        {
            if (string.IsNullOrWhiteSpace(genre)) return;
            Genres.Remove(genre.Trim());
        }

        public void SetGenres(IEnumerable<string>? genres)
        {
            Genres = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (genres == null) return;
            foreach (var g in genres)
            {
                if (string.IsNullOrWhiteSpace(g)) continue;
                Genres.Add(StringNormalizer.Normalize(g)!);
            }
        }
    }
}

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

            var a = new Author
            {
                Name = input.Name.Trim(),
                Bio = string.IsNullOrWhiteSpace(input.Bio) ? null : input.Bio.Trim(),
                Nationality = (input.Nationality ?? string.Empty).Trim(),
                BirthDate = input.BirthDate,
                DeathDate = input.DeathDate
            };

            if (input.Genres != null)
            {
                foreach (var g in input.Genres)
                {
                    if (string.IsNullOrWhiteSpace(g)) continue;
                    a.Genres.Add(g.Trim());
                }
            }

            return a;
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Name is required");
            Name = name.Trim();
        }

        public void SetBio(string? bio)
        {
            Bio = string.IsNullOrWhiteSpace(bio) ? null : bio.Trim();
        }

        public void SetNationality(string nationality)
        {
            Nationality = (nationality ?? string.Empty).Trim();
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
            Genres.Add(genre.Trim());
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
                Genres.Add(g.Trim());
            }
        }
    }
}

using Domain.Models;
using Domain.Common;

namespace Tests.Domain
{
    public class AuthorTests
    {
        [Fact]
        public void Create_ShouldReturnAuthor_WhenValidData()
        {
            var input = new AuthorData
            {
                Name = "J. R. R. Tolkien",
                Bio = "Author of The Lord of the Rings",
                Nationality = "British",
                BirthDate = new DateOnly(1892, 1, 3),
                DeathDate = new DateOnly(1973, 9, 2),
                Genres = new[] { "Fantasy", "Myth" }
            };

            var a = Author.Create(input);

            Assert.NotNull(a);
            Assert.Equal("J. R. R. Tolkien", a.Name);
            Assert.Equal("Author of The Lord of the Rings", a.Bio);
            Assert.Equal("British", a.Nationality);
            Assert.Equal(new DateOnly(1892, 1, 3), a.BirthDate);
            Assert.Equal(new DateOnly(1973, 9, 2), a.DeathDate);
            Assert.Contains("Fantasy", a.Genres);
            Assert.Contains("Myth", a.Genres);
        }

        [Fact]
        public void Create_ShouldThrowDomainException_WhenInputIsNull()
        {
            Assert.Throws<DomainException>(() => Author.Create(null!));
        }

        [Fact]
        public void Create_ShouldThrowDomainException_WhenDeathBeforeBirth()
        {
            var input = new AuthorData
            {
                Name = "Some",
                BirthDate = new DateOnly(2000, 1, 1),
                DeathDate = new DateOnly(1999, 1, 1),
                Nationality = "Unknown",
                Genres = new[] { "General" }
            };

            Assert.Throws<DomainException>(() => Author.Create(input));
        }

        [Fact]
        public void SetBirthAndDeathDates_ShouldValidateOrder_WhenDatesAreSet()
        {
            var input = new AuthorData { Name = "X", Nationality = "Unknown", Genres = new[] { "General" } };
            var a = Author.Create(input);

            a.SetBirthDate(new DateOnly(1900, 1, 1));
            a.SetDeathDate(new DateOnly(1950, 1, 1));

            Assert.Equal(new DateOnly(1900, 1, 1), a.BirthDate);
            Assert.Equal(new DateOnly(1950, 1, 1), a.DeathDate);

            Assert.Throws<DomainException>(() => a.SetBirthDate(new DateOnly(2000, 1, 1)));
        }

        [Fact]
        public void SetNameBioAndNationality_ShouldTrimValues_WhenCalled()
        {
            var a = Author.Create(new AuthorData { Name = "Initial", Nationality = "Unknown", Genres = new[] { "General" } });
            a.SetName(" New Name ");
            Assert.Equal("New Name", a.Name);

            a.SetBio("  Some bio  ");
            Assert.Equal("Some bio", a.Bio);

            a.SetNationality("  Spain  ");
            Assert.Equal("Spain", a.Nationality);
        }

        [Fact]
        public void AddGenre_ShouldNotAddDuplicate_WhenSameGenreDifferentCase()
        {
            var input = new AuthorData { Name = "G", Nationality = "N", Genres = new[] { "Sci-Fi" } };
            var a = Author.Create(input);

            a.AddGenre("sci-fi");
            Assert.Single(a.Genres);
        }

        [Theory]
        [InlineData(null, "Nationality", new string[] { "Fiction" })]
        [InlineData("   ", "Nationality", new string[] { "Fiction" })]
        [InlineData("Name", null, new string[] { "Fiction" })]
        [InlineData("Name", "   ", new string[] { "Fiction" })]
        [InlineData("Name", "Nationality", null)]
        [InlineData("Name", "Nationality", new string[] { })]
        public void Create_ShouldThrowDomainException_WhenRequiredFieldsMissing(string? name, string? nationality, string[]? genres)
        {
            var input = new AuthorData
            {
                Name = name!,
                Nationality = nationality,
                Genres = genres
            };

            Assert.Throws<DomainException>(() => Author.Create(input));
        }

    }
}

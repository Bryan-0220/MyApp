using Domain.Common;
using Domain.Models;

namespace Tests.Domain
{
    public class ReaderTests
    {
        [Fact]
        public void Create_ValidData_ReturnsReader()
        {
            var data = new ReaderData
            {
                FirstName = " John ",
                LastName = " Doe ",
                Email = " john.doe@example.com ",
                MembershipDate = new DateOnly(2020, 1, 1)
            };

            var r = Reader.Create(data);

            Assert.NotNull(r);
            Assert.Equal("John", r.FirstName);
            Assert.Equal("Doe", r.LastName);
            Assert.Equal("john.doe@example.com", r.Email);
            Assert.Equal(new DateOnly(2020, 1, 1), r.MembershipDate);
        }

        [Fact]
        public void Create_NullInput_ThrowsDomainException()
        {
            Assert.Throws<DomainException>(() => Reader.Create(null!));
        }

        [Theory]
        [InlineData(null, "Last", "a@b.com")]
        [InlineData("First", null, "a@b.com")]
        [InlineData("First", "Last", null)]
        [InlineData("   ", "Last", "a@b.com")]
        [InlineData("First", "   ", "a@b.com")]
        [InlineData("First", "Last", "    ")]
        public void Create_MissingRequiredFields_Throws(string first, string last, string email)
        {
            var data = new ReaderData
            {
                FirstName = first,
                LastName = last,
                Email = email
            };

            data.MembershipDate = new DateOnly(2020, 1, 1);

            Assert.Throws<DomainException>(() => Reader.Create(data));
        }

        [Fact]
        public void Create_MissingMembershipDate_ThrowsDomainException()
        {
            var data = new ReaderData
            {
                FirstName = "A",
                LastName = "B",
                Email = "a@b.com",
                MembershipDate = null
            };

            Assert.Throws<DomainException>(() => Reader.Create(data));
        }

        [Fact]
        public void Create_NormalizesStrings()
        {
            var data = new ReaderData
            {
                FirstName = "  Ana   María  ",
                LastName = "  López ",
                Email = "  ANA.LOPEZ@example.com  ",
                MembershipDate = new DateOnly(2021, 1, 1)
            };

            var r = Reader.Create(data);

            Assert.Equal("Ana María", r.FirstName);
            Assert.Equal("López", r.LastName);
            Assert.Equal("ANA.LOPEZ@example.com", r.Email);
        }
    }
}

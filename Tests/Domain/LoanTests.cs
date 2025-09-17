using System;
using Domain.Common;
using Domain.Models;
using Xunit;

namespace Tests.Domain
{
    public class LoanTests
    {
        [Fact]
        public void Create_ShouldThrowDomainException_WhenInputIsNull()
        {
            Assert.Throws<DomainException>(() => Loan.Create(null!));
        }

        [Fact]
        public void Create_ShouldTrimIdsAndSetDefaults_WhenIdsHaveWhitespace()
        {
            var data = new LoanData
            {
                BookId = "  book-1  ",
                ReaderId = "  reader-1  ",
                LoanDate = DateOnly.FromDateTime(DateTime.UtcNow),
                DueDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1)
            };

            var loan = Loan.Create(data);

            Assert.Equal("book-1", loan.BookId);
            Assert.Equal("reader-1", loan.ReaderId);

            Assert.True(loan.LoanDate <= loan.DueDate);
            Assert.Equal(LoanStatus.Active, loan.Status);
        }

        // consolidated required-field tests above

        [Fact]
        public void Create_ShouldAllowEqualLoanAndDueDate_AndSetStatusActive()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var data = new LoanData
            {
                BookId = "book-1",
                ReaderId = "reader-1",
                LoanDate = today,
                DueDate = today
            };

            var loan = Loan.Create(data);

            Assert.Equal(today, loan.LoanDate);
            Assert.Equal(today, loan.DueDate);
            Assert.False(string.IsNullOrWhiteSpace(loan.Id));
            Assert.Equal(LoanStatus.Active, loan.Status);
        }

        [Theory]
        [InlineData(-1, "book-1", "reader-1")]
        [InlineData(0, "    ", "  reader-1  ")]
        [InlineData(1, "book-1", "   ")]
        public void Create_ShouldThrowDomainException_WhenDueDateInvalidOrIdsMissing(int dueOffsetDays, string bookId, string readerId)
        {
            var loanDate = DateOnly.FromDateTime(DateTime.UtcNow);
            var data = new LoanData
            {
                BookId = bookId,
                ReaderId = readerId,
                LoanDate = loanDate,
                DueDate = loanDate.AddDays(dueOffsetDays)
            };

            Assert.Throws<DomainException>(() => Loan.Create(data));
        }

    }
}

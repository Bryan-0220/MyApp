namespace Application.Filters
{
    public class LoanFilter
    {
        public string? UserId { get; init; }
        public string? BookId { get; init; }
        public bool? Returned { get; init; }
        public DateOnly? LoanDate { get; init; }
        public DateOnly? DueDate { get; init; }
        public DateOnly? ReturnedDate { get; init; }

        public LoanFilter() { }

        private LoanFilter(string? userId, string? bookId, bool? returned, DateOnly? loanDate, DateOnly? dueDate, DateOnly? returnedDate)
        {
            UserId = userId;
            BookId = bookId;
            Returned = returned;
            LoanDate = loanDate;
            DueDate = dueDate;
            ReturnedDate = returnedDate;
        }

        public static LoanFilter Create(string? userId = null, string? bookId = null, DateOnly? loanDate = null, DateOnly? dueDate = null, DateOnly? returnedDate = null, string? status = null)
        {
            var cleanedUserId = string.IsNullOrWhiteSpace(userId) ? null : userId.Trim();
            var cleanedBookId = string.IsNullOrWhiteSpace(bookId) ? null : bookId.Trim();

            bool? returned = null;
            if (!string.IsNullOrWhiteSpace(status))
            {
                returned = status == "Returned" ? true : status == "Active" ? false : (bool?)null;
            }

            if (!loanDate.HasValue || !dueDate.HasValue)
            {
                throw new ArgumentException("Both LoanDate and DueDate are required to filter loans.");
            }

            return new LoanFilter(cleanedUserId, cleanedBookId, returned, loanDate, dueDate, returnedDate);
        }
    }
}

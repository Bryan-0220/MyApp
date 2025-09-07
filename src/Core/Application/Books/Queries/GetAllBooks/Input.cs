using Application.Filters;

namespace GetAllBooks
{
    public class GetAllBooksQueryInput
    {
        public BookFilter? Filter { get; set; }
        public int? Skip { get; set; }
        public int? Take { get; set; }
    }
}

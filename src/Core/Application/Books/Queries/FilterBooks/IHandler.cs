namespace FilterBooks
{
    public interface IFilterBooksQueryHandler
    {
        Task<IEnumerable<FilterBooksQueryOutput>> HandleAsync(FilterBooksQueryInput input, CancellationToken ct = default);
    }
}

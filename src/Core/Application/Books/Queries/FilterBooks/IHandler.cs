namespace FilterBooks
{
    public interface IFilterBooksQueryHandler
    {
        Task<IEnumerable<FilterBooksQueryOutput>> Handle(FilterBooksQueryInput input, CancellationToken ct = default);
    }
}

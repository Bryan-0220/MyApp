namespace FilterAuthors
{
    public interface IFilterAuthorsQueryHandler
    {
        Task<IEnumerable<FilterAuthorsQueryOutput>> HandleAsync(FilterAuthorsQueryInput input, CancellationToken ct = default);
    }
}

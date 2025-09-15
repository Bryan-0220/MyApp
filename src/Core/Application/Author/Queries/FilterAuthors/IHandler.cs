namespace FilterAuthors
{
    public interface IFilterAuthorsQueryHandler
    {
        Task<IEnumerable<FilterAuthorsQueryOutput>> Handle(FilterAuthorsQueryInput input, CancellationToken ct = default);
    }
}

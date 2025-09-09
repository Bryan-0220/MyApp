namespace FilterReaders
{
    public interface IFilterReadersQueryHandler
    {
        Task<IEnumerable<FilterReadersQueryOutput>> HandleAsync(FilterReadersQueryInput input, CancellationToken ct = default);
    }
}

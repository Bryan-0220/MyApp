namespace FilterReaders
{
    public interface IFilterReadersQueryHandler
    {
        Task<IEnumerable<FilterReadersQueryOutput>> Handle(FilterReadersQueryInput input, CancellationToken ct = default);
    }
}

namespace FilterLoans
{
    public interface IFilterLoansQueryHandler
    {
        Task<IEnumerable<FilterLoansQueryOutput>> HandleAsync(FilterLoansQueryInput input, CancellationToken ct = default);
    }
}

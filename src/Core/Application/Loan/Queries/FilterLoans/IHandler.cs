namespace FilterLoans
{
    public interface IFilterLoansQueryHandler
    {
        Task<IEnumerable<FilterLoansQueryOutput>> Handle(FilterLoansQueryInput input, CancellationToken ct = default);
    }
}

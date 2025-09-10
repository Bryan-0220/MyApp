namespace GetAllLoans
{
    public interface IGetAllLoansQueryHandler
    {
        Task<IEnumerable<GetAllLoansQueryOutput>> HandleAsync(GetAllLoansQueryInput query, CancellationToken ct);
    }
}

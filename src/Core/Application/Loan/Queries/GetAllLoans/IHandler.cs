namespace GetAllLoans
{
    public interface IGetAllLoansQueryHandler
    {
        Task<IEnumerable<GetAllLoansQueryOutput>> Handle(GetAllLoansQueryInput query, CancellationToken ct);
    }
}

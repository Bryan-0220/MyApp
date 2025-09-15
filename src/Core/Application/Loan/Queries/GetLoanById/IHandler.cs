namespace GetLoanById
{
    public interface IGetLoanByIdQueryHandler
    {
        Task<GetLoanByIdQueryOutput?> Handle(GetLoanByIdQueryInput query, CancellationToken ct);
    }
}

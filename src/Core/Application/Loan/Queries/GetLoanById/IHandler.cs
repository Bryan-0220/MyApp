namespace GetLoanById
{
    public interface IGetLoanByIdQueryHandler
    {
        Task<GetLoanByIdQueryOutput?> HandleAsync(GetLoanByIdQueryInput query, CancellationToken ct);
    }
}

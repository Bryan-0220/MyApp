namespace DeleteLoan
{
    public interface IDeleteLoanCommandHandler
    {
        Task<DeleteLoanCommandOutput> HandleAsync(DeleteLoanCommandInput input, CancellationToken ct = default);
    }
}

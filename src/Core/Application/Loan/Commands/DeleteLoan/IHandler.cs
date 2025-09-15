namespace DeleteLoan
{
    public interface IDeleteLoanCommandHandler
    {
        Task<DeleteLoanCommandOutput> Handle(DeleteLoanCommandInput input, CancellationToken ct = default);
    }
}

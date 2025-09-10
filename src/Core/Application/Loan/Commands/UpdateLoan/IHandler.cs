namespace UpdateLoan
{
    public interface IUpdateLoanCommandHandler
    {
        Task<UpdateLoanCommandOutput?> HandleAsync(UpdateLoanCommandInput input, CancellationToken ct = default);
    }
}

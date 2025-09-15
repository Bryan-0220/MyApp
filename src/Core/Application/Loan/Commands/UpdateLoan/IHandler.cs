namespace UpdateLoan
{
    public interface IUpdateLoanCommandHandler
    {
        Task<UpdateLoanCommandOutput?> Handle(UpdateLoanCommandInput input, CancellationToken ct = default);
    }
}

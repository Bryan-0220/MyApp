namespace CreateLoan
{
    public interface ICreateLoanCommandHandler
    {
        Task<CreateLoanCommandOutput> HandleAsync(CreateLoanCommandInput input, CancellationToken ct = default);
    }
}

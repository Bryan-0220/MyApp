namespace CreateLoan
{
    public interface ICreateLoanCommandHandler
    {
        Task<CreateLoanCommandOutput> Handle(CreateLoanCommandInput input, CancellationToken ct = default);
    }
}

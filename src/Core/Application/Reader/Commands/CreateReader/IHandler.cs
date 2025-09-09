namespace CreateReader
{
    public interface ICreateReaderCommandHandler
    {
        Task<CreateReaderCommandOutput> HandleAsync(CreateReaderCommandInput input, CancellationToken ct = default);
    }
}

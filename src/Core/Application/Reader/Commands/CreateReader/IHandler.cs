namespace CreateReader
{
    public interface ICreateReaderCommandHandler
    {
        Task<CreateReaderCommandOutput> Handle(CreateReaderCommandInput input, CancellationToken ct = default);
    }
}

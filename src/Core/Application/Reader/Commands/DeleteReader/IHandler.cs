namespace DeleteReader
{
    public interface IDeleteReaderCommandHandler
    {
        Task<DeleteReaderCommandOutput> HandleAsync(DeleteReaderCommandInput input, CancellationToken ct = default);
    }
}

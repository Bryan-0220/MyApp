namespace DeleteReader
{
    public interface IDeleteReaderCommandHandler
    {
        Task<DeleteReaderCommandOutput> Handle(DeleteReaderCommandInput input, CancellationToken ct = default);
    }
}

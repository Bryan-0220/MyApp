namespace UpdateReader
{
    public interface IUpdateReaderCommandHandler
    {
        Task<UpdateReaderCommandOutput?> HandleAsync(UpdateReaderCommandInput input, CancellationToken ct = default);
    }
}

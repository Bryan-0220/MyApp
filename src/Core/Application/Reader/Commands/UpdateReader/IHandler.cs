namespace UpdateReader
{
    public interface IUpdateReaderCommandHandler
    {
        Task<UpdateReaderCommandOutput?> Handle(UpdateReaderCommandInput input, CancellationToken ct = default);
    }
}

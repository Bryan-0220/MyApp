namespace UpdateBook
{
    public interface IUpdateBookCommandHandler
    {
        Task<UpdateBookCommandOutput?> HandleAsync(UpdateBookCommandInput input, CancellationToken ct = default);
    }
}
